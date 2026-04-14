# Migration Map: C# to Go

## Scope

Source projects:
- `Indusoft.TM.COM.Base`
- `Indusoft.TM.COM.DLL`
- `Indusoft.TM.COM.Service`

Target projects:
- `cmd/itmcomd`
- `internal/itmcom`

## Non-Functional Exclusions

The following are excluded from functional migration:
- `obj/**`
- `AssemblyInfo.cs`
- `Properties/Resources.cs`
- `Properties/Settings.cs`
- `Properties/Resource.cs`

## Must-Match Behavior Checklist

- Listener lifecycle and modem handshake behavior
- Legacy parser framing and malformed recovery semantics
- ATSWP parser framing/stuffing and queue split semantics
- Runtime CRUD and dynamic reload semantics
- Compat API contract parity (connect/ownership/queues/send)
- MSSQL read/write paths with legacy compatibility mode
- Logging side effects parity (`InsertLog` payload/status)

## Source to Target Mapping

### Indusoft.TM.COM.Base

| Source | Target (Go) | Status |
|---|---|---|
| `ATSWPBatchType.cs` | `internal/itmcom/types.go`, `internal/itmcom/parser_atswp.go` | mapped |
| `BatchData.cs` | `internal/itmcom/types.go` (`BatchRecord`) | mapped |
| `ConfigType.cs` | `internal/itmcom/types.go`, `internal/itmcom/config.go` | mapped |
| `Direction.cs` | `internal/itmcom/types.go` | mapped |
| `EventLogger.cs` | `internal/itmcom/store_mssql.go`, std `log` usage | mapped |
| `IAnComBase.cs` | `internal/itmcom/server.go` + `internal/itmcom/http_api.go` compat layer | mapped |
| `ITMCOMDataSet.cs` | `internal/itmcom/server.go` (`DataSetView`) | mapped |
| `ModemType.cs` | `internal/itmcom/types.go`, `internal/itmcom/config.go` | mapped |
| `ModemUpdateFlag.cs` | `internal/itmcom/types.go` | mapped |
| `NewLogs.cs` | `internal/itmcom/store.go`, `internal/itmcom/store_mssql.go` | mapped |
| `NewModems.cs` | `internal/itmcom/types.go`, `internal/itmcom/server.go` | mapped |
| `SendTo.cs` | `internal/itmcom/types.go` | mapped |
| `StringCommandType.cs` | `internal/itmcom/http_api.go` compat endpoints | mapped |
| `TCPIPClient.cs` | `internal/itmcom/types.go`, `internal/itmcom/server.go` | mapped |
| `UpdatedModems.cs` | `internal/itmcom/types.go`, `internal/itmcom/server.go` | mapped |
| `Utils.cs` | `internal/itmcom/codec.go` | mapped |

### Indusoft.TM.COM.DLL

| Source | Target (Go) | Status |
|---|---|---|
| `ITMCOMDLL.cs` | `internal/itmcom/server.go`, `internal/itmcom/http_api.go` | mapped |
| `ClientArgs.cs` | `internal/itmcom/http_api.go` compat request DTOs | mapped |
| `ListenerArgs.cs` | `internal/itmcom/config.go`, `internal/itmcom/http_api.go` | mapped |
| `DataConvert.cs` | `internal/itmcom/codec.go` | mapped |
| `DBRecord.cs` | `internal/itmcom/store.go`, `internal/itmcom/store_mssql.go` | mapped |
| `LogRecord.cs` | `internal/itmcom/store.go`, `internal/itmcom/store_mssql.go` | mapped |
| `LogStatus.cs` | `internal/itmcom/store_mssql.go` (legacy log status mapping) | mapped |
| `ModemInfo.cs` | `internal/itmcom/types.go` (`ModemState`) | mapped |
| `ModemRecord.cs` | `internal/itmcom/types.go`, `internal/itmcom/server.go` | mapped |
| `MonitorInfo.cs` | `internal/itmcom/types.go`, `internal/itmcom/queue.go` | mapped |
| `NewModemRecord.cs` | `internal/itmcom/types.go` | mapped |
| `PortInfo.cs` | `internal/itmcom/config.go` (`COMPortConfig`) | mapped |
| `ReadModemObject.cs` | `internal/itmcom/http_api.go` read handlers | mapped |
| `RecordType.cs` | `internal/itmcom/types.go` | mapped |
| `Redistribution.cs` | `internal/itmcom/server.go`, `internal/itmcom/com.go` | mapped |
| `Routing.cs` | `internal/itmcom/config.go`, `internal/itmcom/server.go` | mapped |
| `StatisticsRecord.cs` | `internal/itmcom/types.go`, `internal/itmcom/store_mssql.go` | mapped |
| `DynamicRecord.cs` | `internal/itmcom/server.go` (`DataSetView` dynamic projection) | mapped |
| `DynamicTable.cs` | `internal/itmcom/server.go` (`DataSetView`) | mapped |
| `StaticRecord.cs` | `internal/itmcom/types.go` | mapped |
| `StaticTable.cs` | `internal/itmcom/types.go` | mapped |
| `Utilities.cs` | `internal/itmcom/codec.go`, `internal/itmcom/http_api.go` helpers | mapped |
| `WriteObject.cs` | `internal/itmcom/http_api.go` write handlers | mapped |
| `WriteModemInfo.cs` | `internal/itmcom/http_api.go` compat command handlers | mapped |

### Indusoft.TM.COM.Service

| Source | Target (Go) | Status |
|---|---|---|
| `Program.cs` | `cmd/itmcomd/main.go` | mapped |
| `ITMCOMService.cs` | `cmd/itmcomd/main.go`, `internal/itmcom/server.go` | mapped |
| `AnCOMServiceInstaller.cs` | `cmd/itmcomd/main.go` + deployment/runbook responsibilities | mapped |

## Phase 0 Completion

- Inventory complete: yes
- Functional vs non-functional classification complete: yes
- Source-to-target map complete for all functional files: yes
- Must-match behavior checklist captured: yes

## Phase 6 Final Closure

- Migration map revalidated against current Go implementation: yes
- Must-match behavior checklist revalidated with tests/docs evidence: yes
- Open functional gaps within scoped baseline: none
- Final parity report synchronized with closure evidence: yes
