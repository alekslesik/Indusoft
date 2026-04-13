package itmcom

import "testing"

func TestParseLegacyFramesDetailed_MalformedCount(t *testing.T) {
	// Contains embedded 0x0A after first byte to trigger malformed branch.
	receive := []byte{0x01, 0x0A, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09}
	_, stats := parseLegacyFramesDetailed(receive, SendToCOM, "M1", ModemTypeLegacy, func(frame BatchRecord) {})
	if stats.Malformed == 0 {
		t.Fatalf("expected malformed > 0, got %d", stats.Malformed)
	}
	if stats.ParsedFrames != 0 {
		t.Fatalf("expected parsed frames 0, got %d", stats.ParsedFrames)
	}
}

