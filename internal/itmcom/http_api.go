package itmcom

import (
	"context"
	"encoding/hex"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"net/netip"
	"strconv"
	"strings"
	"sync"
	"time"
)

const (
	headerContentType   = "Content-Type"
	contentTypeJSON     = "application/json"
	errMethodNotAllowed = "method not allowed"
	errCompatClientLock = "client machine is not owner"
)

func (s *Server) StartHTTP(ctx context.Context) error {
	addr := s.cfg.HTTPAddr
	if addr == "" {
		addr = "0.0.0.0"
	}
	mux := s.newHTTPMux(newRateLimiter(s.cfg.APIRateLimitPerMin))

	// Shutdown on context cancel.
	srv := &http.Server{
		Addr:    netJoinHostPort(addr, s.cfg.HTTPPort),
		Handler: mux,
	}

	go func() {
		<-ctx.Done()
		shutdownCtx, cancel := context.WithTimeout(context.Background(), 3*time.Second)
		defer cancel()
		_ = srv.Shutdown(shutdownCtx)
	}()

	go func() {
		_ = srv.ListenAndServe()
	}()

	return nil
}

func (s *Server) newHTTPMux(limiter *rateLimiter) *http.ServeMux {
	mux := http.NewServeMux()

	mux.HandleFunc("/healthz", s.withSecurity(limiter, false, func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set(headerContentType, contentTypeJSON)
		_ = json.NewEncoder(w).Encode(map[string]any{
			"status": "ok",
			"time":   time.Now().UTC(),
		})
	}))

	mux.HandleFunc("/v1/modems", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		writeJSON(w, map[string]any{"modems": s.ModemStates()})
	}))

	mux.HandleFunc("/v1/compat/connect", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		writeJSON(w, map[string]any{"ok": s.ConnectClient(req.MachineName)})
	}))

	mux.HandleFunc("/v1/compat/disconnect", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			MachineName string `json:"machineName"`
		}
		if len(body) > 0 {
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		s.DisconnectClient()
		writeJSON(w, map[string]any{"ok": true})
	}))

	mux.HandleFunc("/v1/compat/break", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			ModemID     string `json:"modemID"`
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		writeJSON(w, map[string]any{"ok": s.Break(req.ModemID)})
	}))

	mux.HandleFunc("/v1/compat/getdataset", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		writeJSON(w, s.DataSetView())
	}))

	mux.HandleFunc("/v1/compat/getusecommand", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		writeJSON(w, map[string]any{"modemID": modemID, "inUse": s.GetUseCommand(modemID)})
	}))

	mux.HandleFunc("/v1/compat/setusecommand", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			ModemID     string `json:"modemID"`
			InUse       bool   `json:"inUse"`
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		if !s.SetUseCommand(strings.TrimSpace(req.ModemID), req.InUse) {
			http.Error(w, "modem not found", http.StatusNotFound)
			return
		}
		writeJSON(w, map[string]any{"ok": true})
	}))

	mux.HandleFunc("/v1/compat/getcommanddata", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		clear := true
		if v := r.URL.Query().Get("clear"); v != "" {
			if b, err := strconv.ParseBool(v); err == nil {
				clear = b
			}
		}
		records := s.GetCommandData(modemID, clear)
		writeJSON(w, map[string]any{"modemID": modemID, "count": len(records), "records": records})
	}))

	mux.HandleFunc("/v1/compat/getuseconfig", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		writeJSON(w, map[string]any{"modemID": modemID, "inUse": s.GetUseConfig(modemID)})
	}))

	mux.HandleFunc("/v1/compat/setuseconfig", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			ModemID     string `json:"modemID"`
			InUse       bool   `json:"inUse"`
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		if !s.SetUseConfig(strings.TrimSpace(req.ModemID), req.InUse) {
			http.Error(w, "modem not found", http.StatusNotFound)
			return
		}
		writeJSON(w, map[string]any{"ok": true})
	}))

	mux.HandleFunc("/v1/compat/getconfigdata", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		clear := true
		if v := r.URL.Query().Get("clear"); v != "" {
			if b, err := strconv.ParseBool(v); err == nil {
				clear = b
			}
		}
		records := s.GetConfigData(modemID, clear)
		writeJSON(w, map[string]any{"modemID": modemID, "count": len(records), "records": records})
	}))

	mux.HandleFunc("/v1/compat/getusecommstate", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		writeJSON(w, map[string]any{"modemID": modemID, "inUse": s.GetUseCommState(modemID)})
	}))

	mux.HandleFunc("/v1/compat/setusecommstate", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			ModemID     string `json:"modemID"`
			InUse       bool   `json:"inUse"`
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		if !s.SetUseCommState(strings.TrimSpace(req.ModemID), req.InUse) {
			http.Error(w, "modem not found", http.StatusNotFound)
			return
		}
		writeJSON(w, map[string]any{"ok": true})
	}))

	mux.HandleFunc("/v1/compat/getcommstatedata", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
		if modemID == "" {
			http.Error(w, "modemId is required", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(r.URL.Query().Get("machineName")) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		clear := true
		if v := r.URL.Query().Get("clear"); v != "" {
			if b, err := strconv.ParseBool(v); err == nil {
				clear = b
			}
		}
		records := s.GetCommStateData(modemID, clear)
		writeJSON(w, map[string]any{"modemID": modemID, "count": len(records), "records": records})
	}))

	mux.HandleFunc("/v1/compat/docommand", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "invalid body", http.StatusBadRequest)
			return
		}
		var req struct {
			ModemID     string `json:"modemID"`
			Hex         string `json:"hex"`
			WrapATSWP   *bool  `json:"wrapATSWP"`
			BatchType   *int   `json:"batchType"`
			MachineName string `json:"machineName"`
		}
		if err := json.Unmarshal(body, &req); err != nil {
			http.Error(w, "invalid json", http.StatusBadRequest)
			return
		}
		if !s.IsClientAllowed(req.MachineName) {
			http.Error(w, errCompatClientLock, http.StatusConflict)
			return
		}
		payload, wrap, bt, err := parseHexPayload(body)
		if err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		if req.WrapATSWP != nil {
			wrap = *req.WrapATSWP
		}
		if req.BatchType != nil {
			if *req.BatchType < 0 || *req.BatchType > 255 {
				http.Error(w, "batchType must be 0..255", http.StatusBadRequest)
				return
			}
			bt = byte(*req.BatchType)
		}
		if err := s.SendToModem(strings.TrimSpace(req.ModemID), payload, wrap, bt); err != nil {
			http.Error(w, err.Error(), http.StatusConflict)
			return
		}
		writeJSON(w, map[string]any{"ok": true})
	}))

	mux.HandleFunc("/v1/metrics", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodGet {
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
			return
		}
		writeJSON(w, s.Metrics())
	}))

	mux.HandleFunc("/v1/runtime", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			writeJSON(w, s.CurrentRuntimeData())
		case http.MethodPut:
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var data RuntimeData
			if err := json.Unmarshal(body, &data); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			if err := s.ReplaceRuntimeData(r.Context(), &data); err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		default:
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
		}
	}))

	mux.HandleFunc("/v1/runtime/listeners", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			writeJSON(w, map[string]any{"listeners": s.CurrentRuntimeData().Listeners})
		case http.MethodPost:
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var req ListenerConfig
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			rd.Listeners = append(rd.Listeners, req)
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		case http.MethodDelete:
			idQ := r.URL.Query().Get("configurationId")
			id, err := strconv.Atoi(idQ)
			if err != nil || id <= 0 {
				http.Error(w, "configurationId is required", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			next := make([]ListenerConfig, 0, len(rd.Listeners))
			for _, l := range rd.Listeners {
				if l.ConfigurationId != id {
					next = append(next, l)
				}
			}
			rd.Listeners = next
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		default:
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
		}
	}))

	mux.HandleFunc("/v1/runtime/routing", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			writeJSON(w, map[string]any{"routing": s.CurrentRuntimeData().Routing})
		case http.MethodPost:
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var req RoutingConfig
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			rd.Routing = append(rd.Routing, req)
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		case http.MethodDelete:
			modemID := strings.TrimSpace(r.URL.Query().Get("modemId"))
			if modemID == "" {
				http.Error(w, "modemId is required", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			next := make([]RoutingConfig, 0, len(rd.Routing))
			for _, x := range rd.Routing {
				if x.ModemID != modemID {
					next = append(next, x)
				}
			}
			rd.Routing = next
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		default:
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
		}
	}))

	mux.HandleFunc("/v1/runtime/comports", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			writeJSON(w, map[string]any{"comPorts": s.CurrentRuntimeData().COMPorts})
		case http.MethodPost:
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var req COMPortConfig
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			rd.COMPorts = append(rd.COMPorts, req)
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		case http.MethodDelete:
			portID, err := strconv.Atoi(r.URL.Query().Get("portId"))
			if err != nil || portID <= 0 {
				http.Error(w, "portId is required", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			next := make([]COMPortConfig, 0, len(rd.COMPorts))
			for _, x := range rd.COMPorts {
				if x.PortID != portID {
					next = append(next, x)
				}
			}
			rd.COMPorts = next
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		default:
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
		}
	}))

	mux.HandleFunc("/v1/runtime/comroutes", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		switch r.Method {
		case http.MethodGet:
			writeJSON(w, map[string]any{"comRoutes": s.CurrentRuntimeData().COMRoutes})
		case http.MethodPost:
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var req COMRouteConfig
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			rd.COMRoutes = append(rd.COMRoutes, req)
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		case http.MethodDelete:
			siteID, err := strconv.Atoi(r.URL.Query().Get("siteId"))
			if err != nil {
				http.Error(w, "siteId is required", http.StatusBadRequest)
				return
			}
			portID, err := strconv.Atoi(r.URL.Query().Get("portId"))
			if err != nil {
				http.Error(w, "portId is required", http.StatusBadRequest)
				return
			}
			rd := s.CurrentRuntimeData()
			next := make([]COMRouteConfig, 0, len(rd.COMRoutes))
			for _, x := range rd.COMRoutes {
				if !(x.SiteID == siteID && x.PortID == portID) {
					next = append(next, x)
				}
			}
			rd.COMRoutes = next
			if err := s.ReplaceRuntimeData(r.Context(), rd); err != nil {
				http.Error(w, err.Error(), http.StatusBadRequest)
				return
			}
			writeJSON(w, map[string]any{"status": "ok"})
		default:
			http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
		}
	}))

	mux.HandleFunc("/v1/modems/", s.withSecurity(limiter, true, func(w http.ResponseWriter, r *http.Request) {
		// Path: /v1/modems/{id}/monitor|send|state|disconnect|command|config|commstate/*
		modemID, action, subAction, ok := parseModemPath(r.URL.Path)
		if !ok {
			http.Error(w, "not found", http.StatusNotFound)
			return
		}
		switch action {
		case "monitor":
			if r.Method != http.MethodGet {
				http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
				return
			}
			clear := false
			if v := r.URL.Query().Get("clear"); v != "" {
				if b, err := strconv.ParseBool(v); err == nil {
					clear = b
				}
			}

			s.mu.RLock()
			route := s.routingByModem[modemID]
			s.mu.RUnlock()
			if route == nil {
				http.Error(w, "modem not found", http.StatusNotFound)
				return
			}

			records := route.Monitor.Snapshot(clear)
			w.Header().Set(headerContentType, contentTypeJSON)
			_ = json.NewEncoder(w).Encode(map[string]any{
				"modemID": modemID,
				"count":   len(records),
				"records": records,
			})
		case "state":
			if r.Method != http.MethodGet {
				http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
				return
			}
			var state *ModemState
			for _, st := range s.ModemStates() {
				if st.ModemID == modemID {
					x := st
					state = &x
					break
				}
			}
			if state == nil {
				http.Error(w, "modem not found", http.StatusNotFound)
				return
			}
			w.Header().Set(headerContentType, contentTypeJSON)
			_ = json.NewEncoder(w).Encode(state)
		case "send":
			if r.Method != http.MethodPost {
				http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
				return
			}
			body, err := io.ReadAll(r.Body)
			if err != nil {
				http.Error(w, "invalid body", http.StatusBadRequest)
				return
			}
			var req struct {
				Hex       string `json:"hex"`
				WrapATSWP *bool  `json:"wrapATSWP"`
				BatchType *int   `json:"batchType"`
			}
			if err := json.Unmarshal(body, &req); err != nil {
				http.Error(w, "invalid json", http.StatusBadRequest)
				return
			}
			hexStr := strings.ReplaceAll(strings.TrimSpace(req.Hex), " ", "")
			if hexStr == "" {
				http.Error(w, "hex is required", http.StatusBadRequest)
				return
			}
			payload, err := hex.DecodeString(hexStr)
			if err != nil {
				http.Error(w, "hex decode failed", http.StatusBadRequest)
				return
			}

			wrap := false
			if req.WrapATSWP != nil {
				wrap = *req.WrapATSWP
			}
			bt := byte(0)
			if req.BatchType != nil {
				if *req.BatchType < 0 || *req.BatchType > 255 {
					http.Error(w, "batchType must be 0..255", http.StatusBadRequest)
					return
				}
				bt = byte(*req.BatchType)
			}

			if err := s.SendToModem(modemID, payload, wrap, bt); err != nil {
				http.Error(w, err.Error(), http.StatusConflict)
				return
			}
			w.Header().Set(headerContentType, contentTypeJSON)
			_ = json.NewEncoder(w).Encode(map[string]any{
				"status":  "queued",
				"modemID": modemID,
				"bytes":   len(payload),
				"wrapped": wrap,
			})
		case "disconnect":
			if r.Method != http.MethodPost {
				http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
				return
			}
			if !s.DisconnectModem(modemID) {
				http.Error(w, "modem not connected", http.StatusConflict)
				return
			}
			w.Header().Set(headerContentType, contentTypeJSON)
			_ = json.NewEncoder(w).Encode(map[string]any{"status": "disconnected", "modemID": modemID})
			return
		case "command", "config", "commstate":
			if subAction == "" {
				http.Error(w, "not found", http.StatusNotFound)
				return
			}
			switch subAction {
			case "use":
				if r.Method == http.MethodGet {
					inUse := false
					if action == "command" {
						inUse = s.GetUseCommand(modemID)
					} else if action == "config" {
						inUse = s.GetUseConfig(modemID)
					} else {
						inUse = s.GetUseCommState(modemID)
					}
					writeJSON(w, map[string]any{"modemID": modemID, "inUse": inUse})
					return
				}
				if r.Method != http.MethodPost {
					http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
					return
				}
				body, err := io.ReadAll(r.Body)
				if err != nil {
					http.Error(w, "invalid body", http.StatusBadRequest)
					return
				}
				var req struct {
					InUse bool `json:"inUse"`
				}
				if err := json.Unmarshal(body, &req); err != nil {
					http.Error(w, "invalid json", http.StatusBadRequest)
					return
				}
				ok := false
				if action == "command" {
					ok = s.SetUseCommand(modemID, req.InUse)
				} else if action == "config" {
					ok = s.SetUseConfig(modemID, req.InUse)
				} else {
					ok = s.SetUseCommState(modemID, req.InUse)
				}
				if !ok {
					http.Error(w, "modem not found", http.StatusNotFound)
					return
				}
				writeJSON(w, map[string]any{"status": "ok", "modemID": modemID, "inUse": req.InUse})
				return
			case "data":
				if r.Method != http.MethodGet {
					http.Error(w, errMethodNotAllowed, http.StatusMethodNotAllowed)
					return
				}
				clear := false
				if v := r.URL.Query().Get("clear"); v != "" {
					if b, err := strconv.ParseBool(v); err == nil {
						clear = b
					}
				}
				var records []BatchRecord
				if action == "command" {
					records = s.GetCommandData(modemID, clear)
				} else if action == "config" {
					records = s.GetConfigData(modemID, clear)
				} else {
					records = s.GetCommStateData(modemID, clear)
				}
				writeJSON(w, map[string]any{"modemID": modemID, "count": len(records), "records": records})
				return
			default:
				http.Error(w, "not found", http.StatusNotFound)
				return
			}
		default:
			http.Error(w, "not found", http.StatusNotFound)
			return
		}
	}))

	return mux
}

func (s *Server) withSecurity(limiter *rateLimiter, requireAuth bool, next http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ip := clientIP(r)
		if !limiter.Allow(ip) {
			http.Error(w, "rate limit exceeded", http.StatusTooManyRequests)
			return
		}
		if requireAuth && strings.TrimSpace(s.cfg.APIToken) != "" {
			auth := strings.TrimSpace(r.Header.Get("Authorization"))
			want := "Bearer " + s.cfg.APIToken
			if auth != want {
				http.Error(w, "unauthorized", http.StatusUnauthorized)
				return
			}
		}
		next(w, r)
	}
}

func clientIP(r *http.Request) string {
	hostPort := strings.TrimSpace(r.RemoteAddr)
	if hostPort == "" {
		return "unknown"
	}
	if p, err := netip.ParseAddrPort(hostPort); err == nil {
		return p.Addr().String()
	}
	if p, err := netip.ParseAddr(hostPort); err == nil {
		return p.String()
	}
	return hostPort
}

type rateLimiter struct {
	mu sync.Mutex
	maxPerMinute int
	buckets map[string]*rateBucket
}

type rateBucket struct {
	windowStart time.Time
	count int
}

func newRateLimiter(maxPerMinute int) *rateLimiter {
	if maxPerMinute <= 0 {
		maxPerMinute = 300
	}
	return &rateLimiter{
		maxPerMinute: maxPerMinute,
		buckets: make(map[string]*rateBucket),
	}
}

func (l *rateLimiter) Allow(key string) bool {
	now := time.Now()
	l.mu.Lock()
	defer l.mu.Unlock()
	b := l.buckets[key]
	if b == nil {
		l.buckets[key] = &rateBucket{windowStart: now, count: 1}
		return true
	}
	if now.Sub(b.windowStart) >= time.Minute {
		b.windowStart = now
		b.count = 1
		return true
	}
	if b.count >= l.maxPerMinute {
		return false
	}
	b.count++
	return true
}

func parseModemPath(path string) (modemID, action, subAction string, ok bool) {
	trimmed := strings.TrimPrefix(path, "/v1/modems/")
	parts := strings.Split(trimmed, "/")
	if len(parts) < 2 {
		return "", "", "", false
	}
	modemID = parts[0]
	action = parts[1]
	if modemID == "" || action == "" {
		return "", "", "", false
	}
	if len(parts) >= 3 {
		subAction = parts[2]
	}
	return modemID, action, subAction, true
}

func writeJSON(w http.ResponseWriter, v any) {
	w.Header().Set(headerContentType, contentTypeJSON)
	_ = json.NewEncoder(w).Encode(v)
}

func parseHexPayload(body []byte) ([]byte, bool, byte, error) {
	var req struct {
		Hex       string `json:"hex"`
		WrapATSWP *bool  `json:"wrapATSWP"`
		BatchType *int   `json:"batchType"`
	}
	if err := json.Unmarshal(body, &req); err != nil {
		return nil, false, 0, fmt.Errorf("invalid json")
	}
	hexStr := strings.ReplaceAll(strings.TrimSpace(req.Hex), " ", "")
	if hexStr == "" {
		return nil, false, 0, fmt.Errorf("hex is required")
	}
	payload, err := hex.DecodeString(hexStr)
	if err != nil {
		return nil, false, 0, fmt.Errorf("hex decode failed")
	}

	wrap := false
	if req.WrapATSWP != nil {
		wrap = *req.WrapATSWP
	}
	bt := byte(0)
	if req.BatchType != nil {
		if *req.BatchType < 0 || *req.BatchType > 255 {
			return nil, false, 0, fmt.Errorf("batchType must be 0..255")
		}
		bt = byte(*req.BatchType)
	}
	return payload, wrap, bt, nil
}

func netJoinHostPort(host string, port int) string {
	if strings.Contains(host, ":") {
		// Probably host already has port.
		return host
	}
	return host + ":" + strconv.Itoa(port)
}

