package itmcom

import (
	"bytes"
	"encoding/json"
	"net/http"
	"net/http/httptest"
	"testing"
)

func authReq(t *testing.T, method, url, token string, body any) *http.Request {
	t.Helper()
	var buf bytes.Buffer
	if body != nil {
		if err := json.NewEncoder(&buf).Encode(body); err != nil {
			t.Fatalf("encode body: %v", err)
		}
	}
	req := httptest.NewRequest(method, url, &buf)
	req.Header.Set("Authorization", "Bearer "+token)
	req.Header.Set("Content-Type", "application/json")
	return req
}

func TestHTTPRuntimeCRUDAndCompatFlow(t *testing.T) {
	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	mux := s.newHTTPMux(newRateLimiter(1000))

	// PUT runtime
	runtimeBody := RuntimeData{
		Listeners: []ListenerConfig{
			{ConfigurationId: 1, Address: "127.0.0.1", Port: 9101, ServerID: "S1", ModemType: ModemTypeLegacy},
		},
		Routing: []RoutingConfig{
			{SiteID: 10, ModemID: "M10"},
		},
	}
	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPut, "/v1/runtime", "token123", runtimeBody))
	if rec.Code != http.StatusOK {
		t.Fatalf("PUT /v1/runtime expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// GET runtime/listeners
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/runtime/listeners", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("GET /v1/runtime/listeners expected 200, got %d", rec.Code)
	}

	// compat connect
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{"machineName": "HOST-A"}))
	if rec.Code != http.StatusOK {
		t.Fatalf("POST /v1/compat/connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	// compat getdataset
	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getdataset?machineName=HOST-A", "token123", nil))
	if rec.Code != http.StatusOK {
		t.Fatalf("GET /v1/compat/getdataset expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
}
