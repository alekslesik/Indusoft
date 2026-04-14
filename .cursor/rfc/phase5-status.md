# Phase 5 Status

- focus: `Phase 5 - Protocol hard parity`
- variable: `progress`
- progress: `100%`
- updated: `2026-03-31`

## Notes

- Added parser recovery coverage for split-frame windows (legacy + ATSWP).
- ATSWP parser now preserves incomplete frame tails in `remain` (no false malformed increment).
- Added malformed detection for dangling byte-stuff escape in ATSWP unstuffing.
