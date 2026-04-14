package itmcom

import "testing"

func TestParseLegacyFramesDetailedPartialFrameRemain(t *testing.T) {
	header := []byte{
		0x01,
		0x00, 0x01,
		0x11,
		0x00, 0x02,
		0x00, 0x01,
		0x00,
	}

	partial := append([]byte{}, header...)
	remain, stats := parseLegacyFramesDetailed(partial, SendToCOM, "MODEM1", ModemTypeLegacy, func(frame BatchRecord) {
		// No frame should be emitted for partial payload.
		_ = frame
	})
	if stats.Malformed != 0 {
		t.Fatalf("expected malformed 0 for partial frame, got %d", stats.Malformed)
	}
	if len(remain) != len(partial) {
		t.Fatalf("expected full partial data in remain, got %d", len(remain))
	}

	complete := append(remain, 0x0D)
	got := 0
	remain2, stats2 := parseLegacyFramesDetailed(complete, SendToCOM, "MODEM1", ModemTypeLegacy, func(frame BatchRecord) {
		got++
	})
	if stats2.Malformed != 0 {
		t.Fatalf("expected malformed 0 for complete frame, got %d", stats2.Malformed)
	}
	if got != 1 {
		t.Fatalf("expected exactly one frame, got %d", got)
	}
	if len(remain2) != 0 {
		t.Fatalf("expected empty remain, got %d", len(remain2))
	}
}

