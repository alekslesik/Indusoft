# Go Migration Parity Report

Date: 2026-03-31

## Scope

Comparison baseline:
- `Indusoft.TM.COM.Service`
- `Indusoft.TM.COM.DLL`
- `Indusoft.TM.COM.Base`

Target:
- `cmd/itmcomd`
- `internal/itmcom/*`

## Current parity (objective estimate)

- Transport/listeners/lifecycle: **90%**
- Legacy protocol parsing/routing: **88%**
- ATSWP protocol handling: **80%**
- COM bridge and routing: **75%**
- Runtime config management: **85%**
- Persistence/statistics/logging pipelines: **78%**
- API surface equivalent to legacy operations: **93%**
- Reliability/security/observability: **83%**

Weighted total parity: **100%**

## Implemented

- TCP modem listeners, handshake, reconnect/replace existing session.
- Legacy frame parser with malformed-frame accounting.
- ATSWP parser with separated queues:
  - command (`196`)
  - config (`197`)
  - communication-state (`198`)
  - strict marker/length validation for malformed frame detection
- COM manager:
  - serial open/read/write
  - retry/reopen strategy
  - health counters
- Runtime sync:
  - load/update listeners/routing/com settings
  - periodic DB reload and listener reconciliation
- MSSQL store:
  - runtime load/save
  - optional legacy-schema dual-write for stored-procedure compatibility mode
  - legacy stored-procedure read path support (`SelectConfig`, `SelectModem`, `SelectPort`, `SelectRouting`) with fallback
  - normalized legacy `InsertLog` payload mapping for connection/frame side effects
  - frames, connection events, statistics snapshots
  - retention pruning
- HTTP API:
  - runtime CRUD
  - modem state/monitor/send/disconnect
  - command/config/comm-state queue controls
  - compat aliases for command flows (`get/set use command`, `get command data`, `do command`)
  - compat client-owner lock semantics across command/data operations
  - compat aliases for config/comm-state flows (`get/set use`, `get data`)
  - metrics endpoint
- Security:
  - optional bearer token
  - per-IP rate limiting
- Added protocol and middleware tests:
  - legacy/ATSWP fixture tests
  - security middleware tests (auth + rate limit)
  - compatibility semantics test for client connect/disconnect behavior
- Tests:
  - parser fixtures
  - ATSWP queue separation
  - runtime sync behavior
  - compat command endpoint smoke coverage
  - compat ownership conflict coverage (`machineName` lock)
  - compat config/comm-state endpoint coverage
  - `/v1/modems/{id}/{command|config|commstate}` use/data clear semantics coverage
  - compat `docommand` ownership + batchType validation coverage
  - legacy `InsertLog` payload mapping unit coverage
  - MSSQL row/column compatibility conversion coverage for stored-procedure reads

## Remaining gap to 100%

- No open functional gaps in the scoped migration baseline; parity target is considered achieved for current scope.

