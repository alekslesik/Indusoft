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
		} else {
			applyRuntimeOverrides(cfg, rd)
		}
	}

	srv := itmcom.NewServer(cfg)
	if store != nil {
		srv.SetStore(store)
	}

	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	// Graceful shutdown.
	sigCh := make(chan os.Signal, 1)
	signal.Notify(sigCh, os.Interrupt, syscall.SIGTERM)
	go func() {
		<-sigCh
		log.Printf("shutting down...")
		cancel()
	}()

	if err := startServiceLifecycle(ctx, srv, cfg, store); err != nil {
		log.Fatalf("start lifecycle: %v", err)
	}

	// Block until shutdown.
	<-ctx.Done()
	shutdownCtx, shutdownCancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer shutdownCancel()
	_ = srv.Shutdown(shutdownCtx)
	log.Printf("bye")
	fmt.Println()
}
