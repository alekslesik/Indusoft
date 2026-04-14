package itmcom

import (
	"context"
	"testing"
)

func TestBuildITMCOMDataSetCompat(t *testing.T) {
	s := NewServer(&ServerConfig{QueueSize: 16})
	if err := s.SyncRuntimeData(context.Background(), &RuntimeData{
		Listeners: []ListenerConfig{{ConfigurationId: 1, Address: "127.0.0.1", Port: 8010, ServerID: "S1", ModemType: ModemTypeLegacy}},
		COMPorts:  []COMPortConfig{{PortID: 1, PortName: "COM1", BaudRate: 9600, DataBits: 8, StopBits: 1, Parity: "none"}},
		COMRoutes: []COMRouteConfig{{SiteID: 10, PortID: 1, BeginRange: 1, EndRange: 100, LocalSiteID: 10}},
	}); err != nil {
		t.Fatalf("sync runtime: %v", err)
	}

	ds := BuildITMCOMDataSetCompat(s)
	if len(ds.Tables) != len(legacyDataSetTableNames) {
		t.Fatalf("tables count mismatch: got=%d want=%d", len(ds.Tables), len(legacyDataSetTableNames))
	}
	if _, ok := ds.Tables["Config"]; !ok {
		t.Fatalf("config table missing")
	}
	if _, ok := ds.Tables["Modem"]; !ok {
		t.Fatalf("modem table missing")
	}
}

