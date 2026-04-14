package itmcom

import (
	"net"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatBreakOwnershipAndConnectionParity(t *testing.T) {
	const (
		hostA       = "HOST-A"
		compatBreak = "/v1/compat/break"
	)
	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	s.routingByModem["M1"] = &Routing{
		SiteID:    1,
		ModemID:   "M1",
		MbConnect: true,
		Monitor:   NewMonitorQueue(32),
		Command:   NewMonitorQueue(32),
		Config:    NewMonitorQueue(32),
		CommState: NewMonitorQueue(32),
	}

	c1, c2 := net.Pipe()
	defer c1.Close()
	defer c2.Close()
	s.conns["M1"] = &ModemConn{
		ModemID:   "M1",
		ModemType: ModemTypeATSWP,
		conn:      c1,
		writeCh:   make(chan []byte, 1),
		closeCh:   make(chan struct{}),
	}
	mux := s.newHTTPMux(newRateLimiter(1000))

	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{"machineName": hostA}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatBreak, "token123", map[string]any{
		"modemID":     "M1",
		"machineName": "HOST-B",
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("foreign break expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatBreak, "token123", map[string]any{
		"modemID":     "M1",
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("owner break expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, compatBreak, "token123", map[string]any{
		"modemID":     "M1",
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("break not connected expected 200 with ok=false body, got %d body=%s", rec.Code, rec.Body.String())
	}
}
