package itmcom

import "testing"

func TestRuntimeQueryPlan(t *testing.T) {
	const modern = "SELECT * FROM ITMCOM_Listeners"
	const legacy = "SELECT * FROM Config"
	const proc = "EXECUTE dbo.SelectConfig"

	got := runtimeQueryPlan(true, modern, legacy, proc)
	want := []string{proc, modern, legacy}
	if len(got) != len(want) {
		t.Fatalf("useSP=true plan length mismatch: got=%d want=%d", len(got), len(want))
	}
	for i := range want {
		if got[i] != want[i] {
			t.Fatalf("useSP=true plan mismatch at %d: got=%q want=%q", i, got[i], want[i])
		}
	}

	got = runtimeQueryPlan(false, modern, legacy, proc)
	want = []string{modern, legacy}
	if len(got) != len(want) {
		t.Fatalf("useSP=false plan length mismatch: got=%d want=%d", len(got), len(want))
	}
	for i := range want {
		if got[i] != want[i] {
			t.Fatalf("useSP=false plan mismatch at %d: got=%q want=%q", i, got[i], want[i])
		}
	}
}
