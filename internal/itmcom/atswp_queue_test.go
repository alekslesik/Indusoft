package itmcom

import "testing"

func TestATSWPQueueSeparationByLinkID(t *testing.T) {
	cfg := &ServerConfig{QueueSize: 16}
	s := NewServer(cfg)
	s.routingByModem["M1"] = &Routing{
		SiteID:    1,
		ModemID:   "M1",
		MbConnect: true,
		Monitor:   NewMonitorQueue(16),
		Command:   NewMonitorQueue(16),
		Config:    NewMonitorQueue(16),
		CommState: NewMonitorQueue(16),
	}
	s.routingByModem["M1"].Command.SetInUse(true)
	s.routingByModem["M1"].Config.SetInUse(true)
	s.routingByModem["M1"].CommState.SetInUse(true)

	route := s.routingByModem["M1"]
	push := func(link byte) {
		fr := BatchRecord{ModemID: "M1", LinkID: link, Raw: []byte{1}}
		route.Monitor.Push(fr)
		if fr.LinkID == 196 {
			route.Command.Push(fr)
		}
		if fr.LinkID == 197 {
			route.Config.Push(fr)
		}
		if fr.LinkID == 198 {
			route.CommState.Push(fr)
		}
	}

	push(196)
	push(197)
	push(198)

	if got := route.Command.Len(); got != 1 {
		t.Fatalf("command queue len = %d, want 1", got)
	}
	if got := route.Config.Len(); got != 1 {
		t.Fatalf("config queue len = %d, want 1", got)
	}
	if got := route.CommState.Len(); got != 1 {
		t.Fatalf("commstate queue len = %d, want 1", got)
	}
}

