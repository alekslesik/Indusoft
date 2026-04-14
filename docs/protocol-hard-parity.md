# Protocol Hard Parity (Phase 5)

This phase hardens byte-stream protocol behavior for both legacy and ATSWP parsing paths.

## Implemented

- ATSWP partial-frame handling keeps incomplete data in `remain` without marking it malformed.
- ATSWP byte-unstuffing now reports malformed input when an escape byte has no following byte.
- Added recovery tests for chunk-split frames in both parsers.

## Test Evidence

- `TestParseATSWPFramesDetailedPartialFrameRemain`
- `TestClearByteStaffingDetailedDanglingEscape`
- `TestParseLegacyFramesDetailedPartialFrameRemain`
- `go test ./internal/itmcom`
- `go test ./...`
