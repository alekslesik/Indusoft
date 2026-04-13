package itmcom

import "testing"

func TestLegacyConnectionLogPayload(t *testing.T) {
	msg, status := legacyConnectionLogPayload("M1", true)
	if msg != "MODEM_CONNECTED M1" {
		t.Fatalf("connected message mismatch: %q", msg)
	}
	if status != legacyLogStatusInfo {
		t.Fatalf("connected status mismatch: %d", status)
	}

	msg, status = legacyConnectionLogPayload("M1", false)
	if msg != "MODEM_DISCONNECTED M1" {
		t.Fatalf("disconnected message mismatch: %q", msg)
	}
	if status != legacyLogStatusInfo {
		t.Fatalf("disconnected status mismatch: %d", status)
	}
}

func TestLegacyFrameLogPayload(t *testing.T) {
	msg, status := legacyFrameLogPayload(BatchRecord{
		ModemID:    "M9",
		ModemType:  ModemTypeATSWP,
		SiteIDFrom: 101,
		SiteIDTo:   202,
		LinkID:     196,
		BatchHex:   "AABBCC",
	})

	want := "FRAME modem=M9 type=ATSWP siteFrom=101 siteTo=202 link=196 hex=AABBCC"
	if msg != want {
		t.Fatalf("frame message mismatch:\nwant=%q\ngot=%q", want, msg)
	}
	if status != legacyLogStatusInfo {
		t.Fatalf("frame status mismatch: %d", status)
	}
}

