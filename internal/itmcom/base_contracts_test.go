package itmcom

import (
	"testing"
	"time"
)

func TestBaseEnumParityValues(t *testing.T) {
	if ConfigTypeModem != 1 || ConfigTypeModemATSWP != 4 || ConfigTypeTCPIPClient != 5 {
		t.Fatalf("config type parity mismatch")
	}
	if ModemUpdateFlagNone != 0 || ModemUpdateFlagLastBatch != 3 {
		t.Fatalf("modem update flag parity mismatch")
	}
	if ATSWPBatchTypeCommand != 196 || ATSWPBatchTypeConfig != 197 || ATSWPBatchTypeCommunicationState != 198 {
		t.Fatalf("atswp batch type parity mismatch")
	}
}

func TestBatchDataCompatFromDirection(t *testing.T) {
	at := time.Date(2026, 3, 31, 10, 20, 30, 123000000, time.UTC)
	send := NewBatchDataCompatFromDirection(1, DirectionSend, at, 10, "AA")
	if send.Direction != "send 10" {
		t.Fatalf("send direction mismatch: %q", send.Direction)
	}

	cmd := NewBatchDataCompatFromDirection(1, DirectionReceive, at, -2, "AA")
	if cmd.Direction != batchDirectionCommand {
		t.Fatalf("command direction mismatch: %q", cmd.Direction)
	}
}

func TestBatchDataCompatFromCommand(t *testing.T) {
	at := time.Date(2026, 3, 31, 10, 20, 30, 123000000, time.UTC)
	req := NewBatchDataCompatFromCommand(1, StringCommandTypeRequest, at, -2, "AA")
	if req.Direction != "request" {
		t.Fatalf("request command text mismatch: %q", req.Direction)
	}
	ans := NewBatchDataCompatFromCommand(1, StringCommandTypeAnswer, at, -3, "BB")
	if ans.Direction != "answer" {
		t.Fatalf("answer command text mismatch: %q", ans.Direction)
	}
}
