package main

import (
	"context"
	"testing"

	"indusoft-tm-com-go/internal/itmcom"
)

type fakeServiceController struct {
	syncCalls       int
	startedLns      int
	startHTTPCalls  int
	statsCalls      int
	watchdogCalls   int
	lastRuntimeData *itmcom.RuntimeData
}

func (f *fakeServiceController) SyncRuntimeData(_ context.Context, data *itmcom.RuntimeData) error {
	f.syncCalls++
	f.lastRuntimeData = data
	return nil
}

func (f *fakeServiceController) StartListener(_ context.Context, _ itmcom.ListenerConfig) error {
	f.startedLns++
	return nil
}

func (f *fakeServiceController) StartStatisticsWorker(context.Context) {
	f.statsCalls++
}

func (f *fakeServiceController) StartConnectionWatchdog(context.Context) {
	f.watchdogCalls++
}

func (f *fakeServiceController) StartHTTP(context.Context) error {
	f.startHTTPCalls++
	return nil
}

func (f *fakeServiceController) Shutdown(context.Context) error { return nil }

func TestApplyRuntimeOverrides(t *testing.T) {
	const localhost = "127.0.0.1"

	cfg := &itmcom.ServerConfig{
		Listeners: []itmcom.ListenerConfig{{ConfigurationId: 1, Address: localhost, Port: 8010}},
		Routing:   []itmcom.RoutingConfig{{SiteID: 10, ModemID: "M10"}},
		COMPorts:  []itmcom.COMPortConfig{{PortID: 1, PortName: "COM1"}},
		COMRoutes: []itmcom.COMRouteConfig{{SiteID: 10, PortID: 1}},
	}
	rd := &itmcom.RuntimeData{
		Listeners: []itmcom.ListenerConfig{{ConfigurationId: 2, Address: localhost, Port: 8020}},
		Routing:   []itmcom.RoutingConfig{{SiteID: 20, ModemID: "M20"}},
	}

	applyRuntimeOverrides(cfg, rd)

	if got := cfg.Listeners[0].ConfigurationId; got != 2 {
		t.Fatalf("listeners override mismatch: %d", got)
	}
	if got := cfg.Routing[0].SiteID; got != 20 {
		t.Fatalf("routing override mismatch: %d", got)
	}
	if got := cfg.COMPorts[0].PortName; got != "COM1" {
		t.Fatalf("com ports should stay unchanged on empty runtime list, got %s", got)
	}
}

func TestStartServiceLifecycle(t *testing.T) {
	const localhost = "127.0.0.1"

	cfg := &itmcom.ServerConfig{
		Listeners: []itmcom.ListenerConfig{
			{ConfigurationId: 1, Address: localhost, Port: 8010, ModemType: itmcom.ModemTypeLegacy},
			{ConfigurationId: 2, Address: localhost, Port: 8011, ModemType: itmcom.ModemTypeATSWP},
		},
		HTTPAddr: localhost,
		HTTPPort: 8080,
	}
	fake := &fakeServiceController{}
	if err := startServiceLifecycle(context.Background(), fake, cfg, nil); err != nil {
		t.Fatalf("start service lifecycle failed: %v", err)
	}
	if fake.syncCalls != 1 {
		t.Fatalf("sync calls mismatch: %d", fake.syncCalls)
	}
	if fake.startedLns != 2 {
		t.Fatalf("listeners started mismatch: %d", fake.startedLns)
	}
	if fake.statsCalls != 1 || fake.watchdogCalls != 1 {
		t.Fatalf("workers were not started correctly: stats=%d watchdog=%d", fake.statsCalls, fake.watchdogCalls)
	}
	if fake.startHTTPCalls != 1 {
		t.Fatalf("http start calls mismatch: %d", fake.startHTTPCalls)
	}
}
