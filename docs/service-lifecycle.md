# Service Lifecycle Runbook

## Purpose

This document describes runtime lifecycle behavior for the Go service host (`cmd/itmcomd`) to preserve parity with legacy service orchestration.

## Startup Sequence

1. Load config from `-config` file.
2. If DB store is enabled, load runtime overrides from DB and merge into in-memory config.
3. Initialize server and attach store.
4. Sync runtime data into server (`listeners`, `routing`, `comPorts`, `comRoutes`).
5. Start configured TCP listeners.
6. Start periodic runtime reload worker when store is enabled.
7. Start statistics worker and connection watchdog.
8. Start HTTP API if enabled (`httpPort > 0`).

## Shutdown Sequence

1. Process waits for termination signal (`SIGINT` / `SIGTERM`).
2. Root context is cancelled.
3. Service shutdown is executed with timeout (`5s`).
4. DB store is closed on process exit.

## Operational Notes

- Runtime reload worker runs every 5 seconds and reconciles listener/routing state.
- If DB runtime read fails, service keeps running with previous in-memory config.
- Lifecycle orchestration logic lives in `cmd/itmcomd/lifecycle.go` and is unit-tested.
