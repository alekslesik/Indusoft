package itmcom

import (
	"net"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatDoCommandOwnershipAndValidation(t *testing.T) {
	const (
		hostA           = "HOST-A"
		compatDoCommand = "/v1/compat/docommand"
	)

	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	mux := s.newHTTPMux(newRateLimiter(1000))

	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// Foreign machine must be rejected by owner lock before send attempt.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"modemID":     "M1",
		"hex":         "AA",
		"machineName": "HOST-B",
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("foreign docommand expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	// Owner with invalid batchType should fail validation with 400.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"modemID":     "M1",
		"hex":         "AA",
		"machineName": hostA,
		"batchType":   999,
		"wrapATSWP":   true,
	}))
	if rec.Code != http.StatusBadRequest {
		t.Fatalf("docommand invalid batchType expected 400, got %d body=%s", rec.Code, rec.Body.String())
	}

	// Owner with valid payload but disconnected modem should return conflict.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"modemID":     "M1",
		"hex":         "AA",
		"machineName": hostA,
		"batchType":   196,
		"wrapATSWP":   true,
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("docommand disconnected modem expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	// Connected legacy modem should be rejected by compat command protocol.
	c1, c2 := net.Pipe()
	defer c1.Close()
	defer c2.Close()
	s.conns["M1"] = &ModemConn{
		ModemID:   "M1",
		ModemType: ModemTypeLegacy,
		conn:      c1,
		writeCh:   make(chan []byte, 1),
		closeCh:   make(chan struct{}),
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"modemID":     "M1",
		"hex":         "AA",
		"machineName": hostA,
		"batchType":   196,
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("docommand legacy modem expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	// Connected ATSWP modem should accept and enqueue wrapped command.
	c3, c4 := net.Pipe()
	defer c3.Close()
	defer c4.Close()
	s.conns["M2"] = &ModemConn{
		ModemID:   "M2",
		ModemType: ModemTypeATSWP,
		conn:      c3,
		writeCh:   make(chan []byte, 1),
		closeCh:   make(chan struct{}),
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"modemID":     "M2",
		"hex":         "AA",
		"machineName": hostA,
		"batchType":   196,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("docommand atswp modem expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	select {
	case out := <-s.conns["M2"].writeCh:
		if len(out) == 0 || out[0] != 192 {
			t.Fatalf("expected wrapped ATSWP batch, got %v", out)
		}
	default:
		t.Fatalf("expected command payload queued for M2")
	}

	// Legacy-style identifier + begin/size should be accepted.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"identifier":  "M2",
		"hex":         "AABBCC",
		"begin":       1,
		"size":        1,
		"machineName": hostA,
		"batchType":   196,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("docommand identifier begin/size expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	select {
	case out := <-s.conns["M2"].writeCh:
		if len(out) == 0 || out[0] != 192 {
			t.Fatalf("expected wrapped ATSWP batch for identifier request, got %v", out)
		}
	default:
		t.Fatalf("expected command payload queued for identifier request")
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatDoCommand, "token123", map[string]any{
		"identifier":  "M2",
		"hex":         "AABB",
		"begin":       5,
		"size":        1,
		"machineName": hostA,
		"batchType":   196,
	}))
	if rec.Code != http.StatusBadRequest {
		t.Fatalf("docommand invalid begin/size expected 400, got %d body=%s", rec.Code, rec.Body.String())
	}
}
