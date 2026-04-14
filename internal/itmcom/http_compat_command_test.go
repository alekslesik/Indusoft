package itmcom

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatCommandEndpoints(t *testing.T) {
	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	// Seed routing modem.
	s.routingByModem["M1"] = &Routing{
		SiteID:    1,
		ModemID:   "M1",
		MbConnect: false,
		Monitor:   NewMonitorQueue(32),
		Command:   NewMonitorQueue(32),
		Config:    NewMonitorQueue(32),
		CommState: NewMonitorQueue(32),
	}
	s.routingByModem["M1"].Command.SetInUse(false)
	mux := s.newHTTPMux(newRateLimiter(1000))

	// lock compat client owner
	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{
		"machineName": "HOST-A",
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// setusecommand true
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setusecommand", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       true,
		"machineName": "HOST-A",
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("setusecommand expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// getusecommand
	rec = httptest.NewRecorder()
	req := authReq(t, http.MethodGet, "/v1/compat/getusecommand?modemId=M1&machineName=HOST-A", "token123", nil)
	mux.ServeHTTP(rec, req)
	if rec.Code != http.StatusOK {
		t.Fatalf("getusecommand expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// getcommanddata (empty but should be 200)
	rec = httptest.NewRecorder()
	req = authReq(t, http.MethodGet, "/v1/compat/getcommanddata?modemId=M1&clear=true&machineName=HOST-A", "token123", nil)
	mux.ServeHTTP(rec, req)
	if rec.Code != http.StatusOK {
		t.Fatalf("getcommanddata expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// foreign client must be rejected while owner lock is held.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setusecommand", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       false,
		"machineName": "HOST-B",
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("setusecommand foreign host expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	// case mismatch should be rejected by strict owner check.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getusecommand?modemId=M1&machineName=host-a", "token123", nil))
	if rec.Code != http.StatusConflict {
		t.Fatalf("getusecommand case mismatch expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	// missing modemId must return bad request.
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getcommanddata?machineName=HOST-A", "token123", nil))
	if rec.Code != http.StatusBadRequest {
		t.Fatalf("getcommanddata missing modemId expected 400, got %d body=%s", rec.Code, rec.Body.String())
	}
}
