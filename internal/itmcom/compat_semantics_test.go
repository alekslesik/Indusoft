package itmcom

import "testing"

func TestConnectClientSemantics(t *testing.T) {
	s := NewServer(&ServerConfig{QueueSize: 10})
	if !s.ConnectClient("HOST-A") {
		t.Fatalf("first connect should succeed")
	}
	if !s.ConnectClient("HOST-A") {
		t.Fatalf("same host reconnect should succeed")
	}
	if s.ConnectClient("HOST-B") {
		t.Fatalf("different host should be rejected while locked")
	}
	s.DisconnectClient()
	if !s.ConnectClient("HOST-B") {
		t.Fatalf("after disconnect new host should succeed")
	}
}

