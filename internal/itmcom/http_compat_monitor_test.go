package itmcom

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatMonitorEndpoints(t *testing.T) {
	const hostA = "HOST-A"
	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	s.routingByModem["M1"] = &Routing{
		SiteID:    1,
		ModemID:   "M1",
		MbConnect: false,
		Monitor:   NewMonitorQueue(32),
		Command:   NewMonitorQueue(32),
		Config:    NewMonitorQueue(32),
		CommState: NewMonitorQueue(32),
	}
	s.routingByModem["M1"].Monitor.Push(BatchRecord{ModemID: "M1", LinkID: 1, BatchHex: "AA"})
	mux := s.newHTTPMux(newRateLimiter(1000))

	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{"machineName": hostA}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setusemonitor", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       true,
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("setusemonitor expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getusemonitor?modemId=M1&machineName=HOST-A", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getusemonitor expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getmonitordata?modemId=M1&clear=false&machineName=HOST-A", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getmonitordata clear=false expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	if got := len(s.GetMonitorData("M1", false)); got != 1 {
		t.Fatalf("monitor queue should remain 1 after clear=false, got %d", got)
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getmonitordata?modemId=M1&clear=true&machineName=HOST-A", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getmonitordata clear=true expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	if got := len(s.GetMonitorData("M1", false)); got != 0 {
		t.Fatalf("monitor queue should be 0 after clear=true, got %d", got)
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getusemonitor?modemId=M1&machineName=host-a", "token123", nil))
	if rec.Code != http.StatusConflict {
		t.Fatalf("getusemonitor case mismatch expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}
}
