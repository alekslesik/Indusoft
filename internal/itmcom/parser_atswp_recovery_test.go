package itmcom

import "testing"

func TestParseATSWPFramesDetailedPartialFrameRemain(t *testing.T) {
	// Valid command frame split across chunks.
	chunk1 := []byte{0xC0, 0xC4, 0x00, 0x09, 0x00}
	_, _, remain, stats := parseATSWPFramesDetailed(
		chunk1,
		SendToCOM,
		"M1",
		ModemTypeATSWP,
		nil,
		nil,
		func(fr BatchRecord) {
			// Frame sink is intentionally no-op; this case validates remain behavior.
			_ = fr
		},
	)
	if stats.Malformed != 0 {
		t.Fatalf("partial frame should not be malformed, got %d", stats.Malformed)
	}
	if len(remain) == 0 {
		t.Fatalf("expected ATSWP remain for partial frame")
	}

	chunk2 := append(remain, []byte{0x00, 0xAA, 0xBB, 0xC1}...)
	_, _, remain2, stats2 := parseATSWPFramesDetailed(
		chunk2,
		SendToCOM,
		"M1",
		ModemTypeATSWP,
		nil,
		nil,
		func(fr BatchRecord) {
			// Frame payload is not asserted in this recovery-path test.
			_ = fr
		},
	)
	if stats2.Malformed != 0 {
		t.Fatalf("completed frame should not be malformed, got %d", stats2.Malformed)
	}
	if len(remain2) != 0 {
		t.Fatalf("expected no ATSWP remain after complete frame, got %d", len(remain2))
	}
}

func TestClearByteStaffingDetailedDanglingEscape(t *testing.T) {
	if _, ok := clearByteStaffingDetailed([]byte{0xC0, 0xC4, 0xC3}); ok {
		t.Fatalf("expected dangling escape to be malformed")
	}
}

