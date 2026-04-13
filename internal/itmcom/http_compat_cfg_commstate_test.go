package itmcom

import (
	"fmt"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatConfigAndCommStateEndpoints(t *testing.T) {
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
	s.routingByModem["M1"].Config.SetInUse(false)
	s.routingByModem["M1"].CommState.SetInUse(false)
	mux := s.newHTTPMux(newRateLimiter(1000))

	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// config flow
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setuseconfig", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       true,
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("setuseconfig expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, fmt.Sprintf("/v1/compat/getuseconfig?modemId=M1&machineName=%s", hostA), "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getuseconfig expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, fmt.Sprintf("/v1/compat/getconfigdata?modemId=M1&clear=true&machineName=%s", hostA), "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getconfigdata expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// commstate flow
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setusecommstate", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       true,
		"machineName": hostA,
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("setusecommstate expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, fmt.Sprintf("/v1/compat/getusecommstate?modemId=M1&machineName=%s", hostA), "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getusecommstate expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, fmt.Sprintf("/v1/compat/getcommstatedata?modemId=M1&clear=true&machineName=%s", hostA), "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("getcommstatedata expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// owner lock applies
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/setuseconfig", "token123", map[string]any{
		"modemID":     "M1",
		"inUse":       false,
		"machineName": "HOST-B",
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("setuseconfig foreign host expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}
}

