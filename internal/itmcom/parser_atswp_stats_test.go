package itmcom

import "testing"

func TestParseATSWPFramesDetailed_MalformedLength(t *testing.T) {
	// Command frame with invalid declared length in bytes [2],[3].
	in := []byte{0xC0, 0xC4, 0x00, 0x09, 0xAA, 0xBB, 0xC1}
	_, _, _, stats := parseATSWPFramesDetailed(
		in,
		SendToCOM,
		"M2",
		ModemTypeATSWP,
		nil,
		nil,
		func(fr BatchRecord) {},
	)
	if stats.Malformed == 0 {
		t.Fatalf("expected malformed > 0, got %d", stats.Malformed)
	}
	if stats.ParsedFrames != 0 {
		t.Fatalf("expected parsed 0, got %d", stats.ParsedFrames)
	}
}
