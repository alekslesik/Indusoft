package main

import (
	"context"
	"flag"
	"fmt"
	"log"
	"os"
	"os/signal"
	"syscall"
	"time"

	"indusoft-tm-com-go/internal/itmcom"
)

func main() {
	configPath := flag.String("config", "config.json", "Path to config JSON")
	flag.Parse()

	cfg, err := itmcom.LoadConfig(*configPath)
	if err != nil {
		log.Fatalf("load config: %v", err)
	}

	var store itmcom.Store
	if cfg.DBConnString != "" {
		dbStore, err := itmcom.NewMSSQLStore(cfg.DBConnString, cfg.DBAutoMigrate, cfg.DBUseStoredProcedures)
		if err != nil {
			log.Fatalf("init db store: %v", err)
		}
		store = dbStore
		defer func() { _ = dbStore.Close() }()

		rd, err := dbStore.LoadRuntimeData(context.Background())
		if err != nil {
			log.Printf("db runtime load failed, using file config: %v", err)
		} else if rd != nil {
			if len(rd.Listeners) > 0 {
				cfg.Listeners = rd.Listeners
			}
			if len(rd.Routing) > 0 {
				cfg.Routing = rd.Routing
			}
		}
	}

	srv := itmcom.NewServer(cfg)
	if store != nil {
		srv.SetStore(store)
	}

	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()
	_ = srv.SyncRuntimeData(ctx, &itmcom.RuntimeData{
		Listeners: cfg.Listeners,
		Routing:   cfg.Routing,
		COMPorts:  cfg.COMPorts,
		COMRoutes: cfg.COMRoutes,
	})

	// Graceful shutdown.
	sigCh := make(chan os.Signal, 1)
	signal.Notify(sigCh, os.Interrupt, syscall.SIGTERM)
	go func() {
		<-sigCh
		log.Printf("shutting down...")
		cancel()
	}()

	for _, ln := range cfg.Listeners {
		log.Printf("listening on %s:%d (modemType=%s)", ln.Address, ln.Port, ln.ModemType)
		if err := srv.StartListener(ctx, ln); err != nil {
			log.Fatalf("start listener: %v", err)
		}
	}

	if store != nil {
		go func() {
			t := time.NewTicker(5 * time.Second)
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
	srv.StartStatisticsWorker(ctx)
	srv.StartConnectionWatchdog(ctx)

	if cfg.HTTPPort > 0 {
		log.Printf("starting http api on %s:%d", cfg.HTTPAddr, cfg.HTTPPort)
		if err := srv.StartHTTP(ctx); err != nil {
			log.Fatalf("start http api: %v", err)
		}
	}

	// Block until shutdown.
	<-ctx.Done()
	shutdownCtx, shutdownCancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer shutdownCancel()
	_ = srv.Shutdown(shutdownCtx)
	log.Printf("bye")
	fmt.Println()
}
