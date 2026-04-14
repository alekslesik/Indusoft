package itmcom

import (
	"encoding/hex"
	"testing"
)

func TestWrapATSWPBatch_Data(t *testing.T) {
	payload := []byte{0x01, 0xC0, 0x02, 0xC1, 0xC3}
	out := wrapATSWPBatch(payload, 0) // DataUART1
	if len(out) < 8 {
		t.Fatalf("wrapped too short: %d", len(out))
	}
	if out[0] != 0xC0 || out[len(out)-1] != 0xC1 {
		t.Fatalf("invalid frame markers: %s", hex.EncodeToString(out))
	}
}

func TestWrapATSWPBatch_Command(t *testing.T) {
	payload := []byte{0x10, 0x20, 0x30}
	out := wrapATSWPBatch(payload, 196) // command
	if out[0] != 0xC0 || out[1] != 196 || out[len(out)-1] != 0xC1 {
		t.Fatalf("bad command frame: %s", hex.EncodeToString(out))
	}
}
