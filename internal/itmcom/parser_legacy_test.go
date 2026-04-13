package itmcom

import "testing"

func TestParseLegacyFrames_MockFrame(t *testing.T) {
	// Craft a minimal legacy frame:
	// header9 (9 bytes) + end marker 0x0D (13).
	//
	// IMPORTANT: In legacy header decoding, byte 0x10 is treated as an escape byte.
	// So linkId must not equal 0x10.
	receive := []byte{
		0x01, // first (unused by our decoder)
		0x00, 0x01, // sideIdTo => after Convert => 1
		0x11, // linkId
		0x00, 0x02, // sideIdFrom => after Convert => 2
		0x00, 0x01, // counter (unused)
		0x00, // factor
		0x0D, // end marker
	}

	got := 0
	remain := parseLegacyFrames(receive, SendToCOM, "MODEM1", ModemTypeLegacy, func(fr BatchRecord) {
		got++
		if fr.SiteIDFrom != 2 || fr.SiteIDTo != 1 {
			t.Fatalf("unexpected ids: from=%d to=%d", fr.SiteIDFrom, fr.SiteIDTo)
		}
		if fr.LinkID != 0x11 {
			t.Fatalf("unexpected linkId: %d", fr.LinkID)
		}
	})

	if got != 1 {
		t.Fatalf("expected 1 frame, got %d", got)
	}
	if remain != nil {
		t.Fatalf("expected nil remain, got %v", remain)
	}
}

