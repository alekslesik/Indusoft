package itmcom

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestCompatOwnershipLock(t *testing.T) {
	s := NewServer(&ServerConfig{
		QueueSize:          100,
		APIToken:           "token123",
		APIRateLimitPerMin: 1000,
	})
	mux := s.newHTTPMux(newRateLimiter(1000))

	rec := httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/connect", "token123", map[string]any{
		"machineName": "HOST-A",
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("connect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getdataset?machineName=HOST-B", "token123", nil))
	if rec.Code != http.StatusConflict {
		t.Fatalf("foreign getdataset expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodGet, "/v1/compat/getdataset?machineName=host-a", "token123", nil))
	if rec.Code != http.StatusConflict {
		t.Fatalf("case-mismatch getdataset expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/disconnect", "token123", map[string]any{
		"machineName": "HOST-B",
	}))
	if rec.Code != http.StatusConflict {
		t.Fatalf("foreign disconnect expected 409, got %d body=%s", rec.Code, rec.Body.String())
	}

	rec = httptest.NewRecorder()
	mux.ServeHTTP(rec, authReq(t, http.MethodPost, "/v1/compat/disconnect", "token123", map[string]any{
		"machineName": "HOST-A",
	}))
	if rec.Code != http.StatusOK {
		t.Fatalf("owner disconnect expected 200, got %d body=%s", rec.Code, rec.Body.String())
	}
}
