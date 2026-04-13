package itmcom

import (
	"context"
	"testing"
)

func TestSyncRuntimeDataRouting(t *testing.T) {
	cfg := &ServerConfig{
		QueueSize: 100,
	}
	s := NewServer(cfg)
	ctx := context.Background()

	err := s.SyncRuntimeData(ctx, &RuntimeData{
		Routing: []RoutingConfig{
			{SiteID: 10, ModemID: "M10"},
			{SiteID: 20, ModemID: "M20"},
		},
	})
	if err != nil {
		t.Fatalf("sync runtime: %v", err)
	}
	states := s.ModemStates()
	if len(states) != 2 {
		t.Fatalf("expected 2 states, got %d", len(states))
	}

	err = s.SyncRuntimeData(ctx, &RuntimeData{
		Routing: []RoutingConfig{
			{SiteID: 20, ModemID: "M20"},
		},
	})
	if err != nil {
		t.Fatalf("sync runtime 2: %v", err)
	}
	states = s.ModemStates()
	if len(states) != 1 {
		t.Fatalf("expected 1 state, got %d", len(states))
	}
}

