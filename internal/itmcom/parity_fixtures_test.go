package itmcom

import (
	"encoding/hex"
	"os"
	"strings"
	"testing"
)

func readHexFixture(t *testing.T, path string) []byte {
	t.Helper()
	b, err := os.ReadFile(path)
	if err != nil {
		t.Fatalf("read fixture %s: %v", path, err)
	}
	s := strings.ReplaceAll(strings.TrimSpace(string(b)), " ", "")
	s = strings.ReplaceAll(s, "\n", "")
	s = strings.ReplaceAll(s, "\r", "")
	out, err := hex.DecodeString(s)
	if err != nil {
		t.Fatalf("decode fixture %s: %v", path, err)
	}
	return out
}

func TestParityFixture_LegacyFrame(t *testing.T) {
	in := readHexFixture(t, "testdata/legacy_frame.hex")
	var got []BatchRecord
	remain, stats := parseLegacyFramesDetailed(in, SendToCOM, "MODEM1", ModemTypeLegacy, func(fr BatchRecord) {
		got = append(got, fr)
	})
	if len(remain) != 0 {
		t.Fatalf("expected no remain, got %d bytes", len(remain))
	}
	if stats.ParsedFrames != 1 || stats.Malformed != 0 {
		t.Fatalf("unexpected stats: %+v", stats)
	}
	if len(got) != 1 {
		t.Fatalf("expected 1 frame, got %d", len(got))
	}
	if got[0].SiteIDFrom != 2 || got[0].SiteIDTo != 1 || got[0].LinkID != 0x11 {
		t.Fatalf("unexpected decoded frame: %+v", got[0])
	}
}

func TestParityFixture_ATSWPCommandFrame(t *testing.T) {
	in := readHexFixture(t, "testdata/atswp_command_frame.hex")
	var got []BatchRecord
	_, _, _ = parseATSWPFrames(
		in,
		SendToCOM,
		"MODEM2",
		ModemTypeATSWP,
		nil,
		nil,
		func(fr BatchRecord) {
			got = append(got, fr)
		},
	)
	if len(got) != 1 {
		t.Fatalf("expected 1 frame, got %d", len(got))
	}
	if got[0].LinkID != 196 {
		t.Fatalf("expected command LinkID=196, got %d", got[0].LinkID)
	}
	if got[0].SiteIDFrom != -2 || got[0].SiteIDTo != -3 {
		t.Fatalf("unexpected command site markers: from=%d to=%d", got[0].SiteIDFrom, got[0].SiteIDTo)
	}
}
