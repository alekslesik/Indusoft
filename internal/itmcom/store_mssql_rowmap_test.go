package itmcom

import "testing"

func TestAsStringAndAsInt(t *testing.T) {
	m := map[string]any{
		"portnumber": []byte("8010"),
		"modemid":    []byte("M55"),
		"typeid":     int64(4),
	}

	if got := asString(m, "modemid"); got != "M55" {
		t.Fatalf("asString modemid mismatch: %q", got)
	}
	if got := asInt(m, "portnumber"); got != 8010 {
		t.Fatalf("asInt portnumber mismatch: %d", got)
	}
	if got := asInt(m, "typeid"); got != 4 {
		t.Fatalf("asInt typeid mismatch: %d", got)
	}
}

