package itmcom

import (
	"fmt"
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestHTTPModemQueueUseAndDataSemantics(t *testing.T) {
	const modemBase = "/v1/modems/M1/"

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
	s.routingByModem["M1"].Command.Push(BatchRecord{ModemID: "M1", LinkID: 196})
	s.routingByModem["M1"].Config.Push(BatchRecord{ModemID: "M1", LinkID: 197})
	s.routingByModem["M1"].CommState.Push(BatchRecord{ModemID: "M1", LinkID: 198})

	mux := s.newHTTPMux(newRateLimiter(1000))
	actions := []string{"command", "config", "commstate"}

	for _, action := range actions {
		assertUseEndpoint(t, mux, modemBase, action)
		assertDataEndpoint(t, mux, modemBase, action, false)
		assertQueueDepth(t, s, action, 1)
	}

	for _, action := range actions {
		assertDataEndpoint(t, mux, modemBase, action, true)
		assertQueueDepth(t, s, action, 0)
	}
}

func assertUseEndpoint(t *testing.T, mux *http.ServeMux, modemBase, action string) {
	t.Helper()
	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, modemBase+action+"/use", "token123", map[string]any{"inUse": true}))
	if rec.Code != http.StatusOK {
		t.Fatalf("%s use POST expected 200, got %d body=%s", action, rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, modemBase+action+"/use", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("%s use GET expected 200, got %d body=%s", action, rec.Code, rec.Body.String())
	}
}

func assertDataEndpoint(t *testing.T, mux *http.ServeMux, modemBase, action string, clear bool) {
	t.Helper()
	url := fmt.Sprintf("%s%s/data?clear=%t", modemBase, action, clear)
	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, url, "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("%s data clear=%t expected 200, got %d body=%s", action, clear, rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, url, "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("%s data second clear=%t expected 200, got %d body=%s", action, clear, rec.Code, rec.Body.String())
	}
}

func assertQueueDepth(t *testing.T, s *Server, action string, expected int) {
	t.Helper()
	switch action {
	case "command":
		if got := len(s.GetCommandData("M1", false)); got != expected {
			t.Fatalf("command queue expected %d, got %d", expected, got)
		}
	case "config":
		if got := len(s.GetConfigData("M1", false)); got != expected {
			t.Fatalf("config queue expected %d, got %d", expected, got)
		}
	default:
		if got := len(s.GetCommStateData("M1", false)); got != expected {
			t.Fatalf("commstate queue expected %d, got %d", expected, got)
		}
	}
}

