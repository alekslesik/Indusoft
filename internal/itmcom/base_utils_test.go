package itmcom

import "testing"

func TestGetStringCommandTypeCompat(t *testing.T) {
	if got := GetStringCommandTypeCompat(StringCommandTypeRequest); got != "request" {
		t.Fatalf("request mapping mismatch: %q", got)
	}
	if got := GetStringCommandTypeCompat(StringCommandTypeAnswer); got != "answer" {
		t.Fatalf("answer mapping mismatch: %q", got)
	}
	if got := GetStringCommandTypeCompat(StringCommandTypeError); got != "error" {
		t.Fatalf("error mapping mismatch: %q", got)
	}
}

func TestTranslateAndNormalizeCompat(t *testing.T) {
	data := []byte{0xAA, 0xBB, 0xCC}
	if got := TranslateModemBatchCompat(data, 0, 2); got != "AA BB " {
		t.Fatalf("translate mismatch: %q", got)
	}
	if got := TranslateModemBatchCompat(data, 9, 2); got != "" {
		t.Fatalf("translate out-of-range mismatch: %q", got)
	}

	norm := NormalizeNameCompat("MODEM-1")
	if len(norm) != 20 {
		t.Fatalf("normalize length mismatch: %d", len(norm))
	}
}

func TestTCPIPClientCompatParsePoint(t *testing.T) {
	c := NewTCPIPClientCompat(7)
	if err := c.ParsePoint("127.0.0.1:8010"); err != nil {
		t.Fatalf("parse point failed: %v", err)
	}
	if c.Point == nil {
		t.Fatalf("parsed point must not be nil")
	}
	if c.SiteID != 7 {
		t.Fatalf("site id mismatch: %d", c.SiteID)
	}
}

