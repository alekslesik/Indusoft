package itmcom

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func TestWithSecurity_AuthRequired(t *testing.T) {
	s := NewServer(&ServerConfig{
		APIToken:           "secret",
		APIRateLimitPerMin: 100,
	})
	limiter := newRateLimiter(100)

	protected := s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		w.WriteHeader(http.StatusOK)
	})

	req := httptest.NewRequest(http.MethodGet, "/v1/modems", nil)
	rec := httptest.NewRecorder()
	protected(rec, req)
	if rec.Code != http.StatusUnauthorized {
		t.Fatalf("expected 401, got %d", rec.Code)
	}

	req2 := httptest.NewRequest(http.MethodGet, "/v1/modems", nil)
	req2.Header.Set("Authorization", "Bearer secret")
	rec2 := httptest.NewRecorder()
	protected(rec2, req2)
	if rec2.Code != http.StatusOK {
		t.Fatalf("expected 200, got %d", rec2.Code)
	}
}

func TestWithSecurity_RateLimit(t *testing.T) {
	s := NewServer(&ServerConfig{
		APIToken:           "",
		APIRateLimitPerMin: 2,
	})
	limiter := newRateLimiter(2)
	protected := s.withSecurity(limiter, false, func(w http.ResponseWriter, r *http.Request) {
		w.WriteHeader(http.StatusOK)
	})

	req := httptest.NewRequest(http.MethodGet, "/healthz", nil)
	req.RemoteAddr = "127.0.0.1:1234"

	rec1 := httptest.NewRecorder()
	protected(rec1, req)
	if rec1.Code != http.StatusOK {
		t.Fatalf("expected 200 on first call, got %d", rec1.Code)
	}

	rec2 := httptest.NewRecorder()
	protected(rec2, req)
	if rec2.Code != http.StatusOK {
		t.Fatalf("expected 200 on second call, got %d", rec2.Code)
	}

	rec3 := httptest.NewRecorder()
	protected(rec3, req)
	if rec3.Code != http.StatusTooManyRequests {
		t.Fatalf("expected 429 on third call, got %d", rec3.Code)
	}
}

