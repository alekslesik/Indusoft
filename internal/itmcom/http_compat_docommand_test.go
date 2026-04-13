package itmcom

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatDoCommandOwnershipAndValidation(t *testing.T) {
	const (
		hostA          = "HOST-A"
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
}

