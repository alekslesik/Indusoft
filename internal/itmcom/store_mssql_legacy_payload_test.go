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

func TestLegacyParityInt(t *testing.T) {
	cases := []struct {
		in   string
		want int
	}{
		{in: "odd", want: 1},
		{in: "Odd", want: 1},
		{in: "1", want: 1},
		{in: "even", want: 2},
		{in: "Even", want: 2},
		{in: "2", want: 2},
		{in: "none", want: 0},
		{in: "", want: 0},
	}
	for _, tc := range cases {
		if got := legacyParityInt(tc.in); got != tc.want {
			t.Fatalf("legacyParityInt(%q) mismatch: got=%d want=%d", tc.in, got, tc.want)
		}
	}
}

