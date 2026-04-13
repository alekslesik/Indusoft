# RFC: Full Indusoft to Go Migration Plan

## Context

Goal: translate all functionality from:
- `Indusoft.TM.COM.Base`
- `Indusoft.TM.COM.DLL`
- `Indusoft.TM.COM.Service`

into the Go implementation with behavior parity.

## Migration Strategy (Brainstorming)

Instead of migrating file-by-file blindly, execute by behavior contracts:

1. **Contracts and models first** (`Base` parity)
2. **Business logic and compat API** (`DLL` parity)
3. **Service lifecycle and hosting behavior** (`Service` parity)
4. **Database and stored procedure parity**
5. **Protocol edge-case parity** (legacy + ATSWP)
6. **Evidence-based closure** (tests + report)

This reduces regression risk and keeps compatibility measurable.

## Writing Plan

### Phase 0: Baseline and inventory
- Build source-to-target map for every meaningful `.cs` file.
- Exclude non-functional artifacts (`obj`, assembly metadata boilerplate).
- Define must-match behavior checklist.

**Definition of Done:**
- `MIGRATION_MAP.md` exists and covers Base/DLL/Service source set.

### Phase 1: `Base` parity
- Port enums, DTOs, record-like structures, and dataset shape.
- Ensure field naming/serialization compatibility.

**Definition of Done:**
- All `Base` contracts have Go equivalents with unit coverage.

### Phase 2: `DLL` parity (core behavior)
- Close one-to-one semantics for connect/disconnect and ownership.
- Close command/config/commstate queues (`use` flags + data reads).
- Close send/do-command semantics and runtime update flows.

**Definition of Done:**
- Integration coverage exists for each public compatibility scenario.

### Phase 3: `Service` parity
- Reproduce service startup/shutdown behavior.
- Reproduce operational lifecycle semantics from `Program`/service class.

**Definition of Done:**
- Lifecycle tests and operational runbook are available.

### Phase 4: DB + stored procedures parity
- Ensure read path supports modern tables plus legacy SP fallback.
- Ensure write side effects (`InsertLog` payload/status) match legacy semantics.
- Keep legacy dual-write compatibility mode.

**Definition of Done:**
- Integration tests pass in both modern and legacy-compat modes.

### Phase 5: Protocol hard parity
- Expand fixture matrix for legacy and ATSWP byte edge-cases.
- Verify malformed recovery and frame boundary behavior under stress.

**Definition of Done:**
- Parser conformance suite passes on all fixture classes.

### Phase 6: Final closure
- Cross-check source contract map vs implemented Go behavior.
- Update parity report with test evidence and closure checklist.

**Definition of Done:**
- No open functional gaps within scoped migration baseline.
- Full test suite is green.

## Notes

- Prioritize backward compatibility unless a breaking change is explicitly requested.
- Any parity claim must be backed by tests and explicit traceability in the migration map.
