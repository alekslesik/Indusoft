package main

import (
	"context"
	"log"
	"time"

	"indusoft-tm-com-go/internal/itmcom"
)

// serviceController abstracts runtime lifecycle operations used by the service host.
// It is intentionally narrow to keep startup/shutdown orchestration testable.
type serviceController interface {
	SyncRuntimeData(ctx context.Context, data *itmcom.RuntimeData) error
	StartListener(ctx context.Context, cfg itmcom.ListenerConfig) error
	StartStatisticsWorker(ctx context.Context)
	StartConnectionWatchdog(ctx context.Context)
	StartHTTP(ctx context.Context) error
	Shutdown(ctx context.Context) error
}

// buildRuntimeDataFromConfig projects static config into runtime data shape.
func buildRuntimeDataFromConfig(cfg *itmcom.ServerConfig) *itmcom.RuntimeData {
	if cfg == nil {
		return &itmcom.RuntimeData{}
	}
	return &itmcom.RuntimeData{
		Listeners: cfg.Listeners,
		Routing:   cfg.Routing,
		COMPorts:  cfg.COMPorts,
		COMRoutes: cfg.COMRoutes,
	}
}

// applyRuntimeOverrides applies store-loaded runtime data over in-memory config.
// Non-empty slices from DB runtime data have priority over file-based config.
func applyRuntimeOverrides(cfg *itmcom.ServerConfig, rd *itmcom.RuntimeData) {
	if cfg == nil || rd == nil {
		return
	}
	if len(rd.Listeners) > 0 {
		cfg.Listeners = rd.Listeners
	}
	if len(rd.Routing) > 0 {
		cfg.Routing = rd.Routing
	}
	if len(rd.COMPorts) > 0 {
		cfg.COMPorts = rd.COMPorts
	}
	if len(rd.COMRoutes) > 0 {
		cfg.COMRoutes = rd.COMRoutes
	}
}

// startServiceLifecycle starts listeners, workers and optional HTTP endpoint.
func startServiceLifecycle(ctx context.Context, srv serviceController, cfg *itmcom.ServerConfig, store itmcom.Store) error {
	if err := srv.SyncRuntimeData(ctx, buildRuntimeDataFromConfig(cfg)); err != nil {
		return err
	}

	for _, ln := range cfg.Listeners {
		log.Printf("listening on %s:%d (modemType=%s)", ln.Address, ln.Port, ln.ModemType)
		if err := srv.StartListener(ctx, ln); err != nil {
			return err
		}
	}

	if store != nil {
		startRuntimeReloadWorker(ctx, srv, store, 5*time.Second)
	}
	srv.StartStatisticsWorker(ctx)
	srv.StartConnectionWatchdog(ctx)

	if cfg.HTTPPort > 0 {
		log.Printf("starting http api on %s:%d", cfg.HTTPAddr, cfg.HTTPPort)
		if err := srv.StartHTTP(ctx); err != nil {
			return err
		}
	}
	return nil
}

// startRuntimeReloadWorker periodically reloads runtime configuration from store.
func startRuntimeReloadWorker(ctx context.Context, srv serviceController, store itmcom.Store, interval time.Duration) {
	go func() {
		t := time.NewTicker(interval)
		defer t.Stop()
		for {
			select {
			case <-ctx.Done():
				return
			case <-t.C:
				rd, err := store.LoadRuntimeData(ctx)
				if err != nil {
					log.Printf("runtime reload failed: %v", err)
					continue
				}
				if err := srv.SyncRuntimeData(ctx, rd); err != nil {
					log.Printf("runtime sync failed: %v", err)
				}
			}
		}
	}()
}
