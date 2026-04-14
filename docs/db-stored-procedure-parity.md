# DB Stored Procedure Parity

## Scope

Phase 4 parity for MSSQL runtime loading and compatibility write side-effects.

## Runtime Load Strategy

`internal/itmcom/store_mssql.go` uses deterministic fallback plans:

- When `dbUseStoredProcedures=true`:
  1. Try legacy stored procedure (`SelectConfig`, `SelectModem`, `SelectPort`, `SelectRouting`)
  2. Fallback to modern `ITMCOM_*` table query
  3. Fallback to legacy table query

- When `dbUseStoredProcedures=false`:
  1. Try modern `ITMCOM_*` table query
  2. Fallback to legacy table query

This keeps legacy-first behavior where requested without breaking modern schema deployments.

## Write-Side Compatibility

- `SaveRuntimeData` keeps modern writes and optional legacy dual-write.
- `SaveConnectionEvent` and `SaveFrame` can emit legacy `InsertLog` payloads when stored-procedure mode is enabled.
- Legacy port parity mapping is normalized by `legacyParityInt()`.

## Test Evidence

- `store_mssql_query_plan_test.go` validates query-plan ordering.
- `store_mssql_query_runtime_rows_test.go` validates actual SQL call order via `sqlmock`.
- `store_mssql_legacy_payload_test.go` validates `InsertLog` payload and parity mapping helpers.
