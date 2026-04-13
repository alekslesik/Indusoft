package itmcom

import (
	"context"
	"errors"
	"fmt"
	"io"
	"log"
	"net"
	"strconv"
	"strings"
	"sync"
	"sync/atomic"
	"time"
)

type Server struct {
	cfg *ServerConfig
	store Store

	mu sync.RWMutex

	routingByModem map[string]*Routing

	// Active connections, one per modemID.
	conns map[string]*ModemConn

	listeners []net.Listener
	listenerByID map[int]net.Listener
	listenerCfgByID map[int]ListenerConfig
	comManager *COMManager
	comPorts []COMPortConfig
	comRoutes []COMRouteConfig
	startedAt time.Time
	readErrors int64
	writeErrors int64
	parseErrors int64
	droppedFrames int64
	clientMachine string

	shutdownOnce sync.Once
}

func NewServer(cfg *ServerConfig) *Server {
	s := &Server{
		cfg:            cfg,
		store:          &noOpStore{},
		routingByModem: make(map[string]*Routing),
		conns:          make(map[string]*ModemConn),
		listenerByID:   make(map[int]net.Listener),
		listenerCfgByID: make(map[int]ListenerConfig),
		comManager:     NewCOMManager(),
		startedAt:      time.Now(),
	}
	for _, rc := range cfg.Routing {
		route := &Routing{
			SiteID:  rc.SiteID,
			ModemID: rc.ModemID,
			MbConnect: false,
			Monitor: NewMonitorQueue(cfg.QueueSize),
			Command: NewMonitorQueue(cfg.QueueSize),
			Config: NewMonitorQueue(cfg.QueueSize),
			CommState: NewMonitorQueue(cfg.QueueSize),
		}
		route.Command.SetInUse(false)
		route.Config.SetInUse(false)
		route.CommState.SetInUse(false)
		s.routingByModem[rc.ModemID] = route
	}
	return s
}

func (s *Server) SetStore(store Store) {
	if store == nil {
		s.store = &noOpStore{}
		return
	}
	s.store = store
}

func (s *Server) StartListener(ctx context.Context, lnCfg ListenerConfig) error {
	s.mu.RLock()
	if _, exists := s.listenerByID[lnCfg.ConfigurationId]; exists {
		s.mu.RUnlock()
		return nil
	}
	s.mu.RUnlock()

	ln, err := net.Listen("tcp", fmt.Sprintf("%s:%d", lnCfg.Address, lnCfg.Port))
	if err != nil {
		return err
	}

	s.mu.Lock()
	s.listeners = append(s.listeners, ln)
	s.listenerByID[lnCfg.ConfigurationId] = ln
	s.listenerCfgByID[lnCfg.ConfigurationId] = lnCfg
	s.mu.Unlock()

	go func() {
		defer func() {
			_ = ln.Close()
			s.mu.Lock()
			delete(s.listenerByID, lnCfg.ConfigurationId)
			delete(s.listenerCfgByID, lnCfg.ConfigurationId)
			s.mu.Unlock()
		}()

		log.Printf("listener started: %s:%d", lnCfg.Address, lnCfg.Port)

		for {
			select {
			case <-ctx.Done():
				return
			default:
			}

			conn, err := ln.Accept()
			if err != nil {
				if ctx.Err() != nil {
					return
				}
				// temporary error
				if ne, ok := err.(net.Error); ok && ne.Temporary() {
					time.Sleep(100 * time.Millisecond)
					continue
				}
				log.Printf("accept error: %v", err)
				return
			}

			go s.handleModemConnection(ctx, conn, lnCfg)
		}
	}()

	return nil
}

func (s *Server) StopListener(configurationID int) {
	s.mu.Lock()
	ln := s.listenerByID[configurationID]
	delete(s.listenerByID, configurationID)
	delete(s.listenerCfgByID, configurationID)
	s.mu.Unlock()
	if ln != nil {
		_ = ln.Close()
	}
}

func (s *Server) Shutdown(ctx context.Context) error {
	var err error
	s.shutdownOnce.Do(func() {
		s.mu.Lock()
		for id, ln := range s.listenerByID {
			_ = ln.Close()
			delete(s.listenerByID, id)
			delete(s.listenerCfgByID, id)
		}
		s.mu.Unlock()

		// Close active conns.
		s.mu.Lock()
		for _, c := range s.conns {
			close(c.closeCh)
			_ = c.conn.Close()
		}
		s.mu.Unlock()
	})

	// Wait for ctx deadline; no strict synchronization needed for MVP.
	select {
	case <-ctx.Done():
		return errors.New("shutdown timed out")
	default:
		return err
	}
}

func (s *Server) SyncRuntimeData(ctx context.Context, data *RuntimeData) error {
	if data == nil {
		return nil
	}

	// Sync routing map.
	nextRouting := make(map[string]RoutingConfig, len(data.Routing))
	for _, r := range data.Routing {
		nextRouting[r.ModemID] = r
	}
	s.mu.Lock()
	for modemID, route := range s.routingByModem {
		if _, ok := nextRouting[modemID]; !ok {
			delete(s.routingByModem, modemID)
			delete(s.conns, modemID)
			route.Monitor.SetInUse(false)
		}
	}
	for modemID, r := range nextRouting {
		if existing := s.routingByModem[modemID]; existing != nil {
			existing.SiteID = r.SiteID
			continue
		}
		s.routingByModem[modemID] = &Routing{
			SiteID:    r.SiteID,
			ModemID:   r.ModemID,
			MbConnect: false,
			Monitor:   NewMonitorQueue(s.cfg.QueueSize),
			Command:   NewMonitorQueue(s.cfg.QueueSize),
			Config:    NewMonitorQueue(s.cfg.QueueSize),
			CommState: NewMonitorQueue(s.cfg.QueueSize),
		}
		s.routingByModem[modemID].Command.SetInUse(false)
		s.routingByModem[modemID].Config.SetInUse(false)
		s.routingByModem[modemID].CommState.SetInUse(false)
	}
	s.mu.Unlock()

	// Sync listeners.
	target := make(map[int]ListenerConfig, len(data.Listeners))
	for _, l := range data.Listeners {
		target[l.ConfigurationId] = l
	}

	s.mu.RLock()
	current := make(map[int]ListenerConfig, len(s.listenerCfgByID))
	for k, v := range s.listenerCfgByID {
		current[k] = v
	}
	s.mu.RUnlock()

	for id, cfg := range current {
		newCfg, ok := target[id]
		if !ok {
			s.StopListener(id)
			continue
		}
		if cfg != newCfg {
			s.StopListener(id)
		}
	}
	for id, cfg := range target {
		if old, ok := current[id]; ok && old == cfg {
			continue
		}
		if err := s.StartListener(ctx, cfg); err != nil {
			return err
		}
	}

	s.comManager.Sync(ctx, data.COMPorts, data.COMRoutes, func(fr BatchRecord) {
		// COM -> modem forwarding by destination site.
		_ = s.SendToSite(fr.SiteIDTo, fr.Raw)
	})
	s.mu.Lock()
	s.comPorts = append([]COMPortConfig(nil), data.COMPorts...)
	s.comRoutes = append([]COMRouteConfig(nil), data.COMRoutes...)
	s.mu.Unlock()
	return nil
}

func (s *Server) CurrentRuntimeData() *RuntimeData {
	s.mu.RLock()
	defer s.mu.RUnlock()
	out := &RuntimeData{}
	for _, cfg := range s.listenerCfgByID {
		out.Listeners = append(out.Listeners, cfg)
	}
	for _, route := range s.routingByModem {
		out.Routing = append(out.Routing, RoutingConfig{
			SiteID:  route.SiteID,
			ModemID: route.ModemID,
		})
	}
	out.COMPorts = append(out.COMPorts, s.comPorts...)
	out.COMRoutes = append(out.COMRoutes, s.comRoutes...)
	return out
}

func (s *Server) ReplaceRuntimeData(ctx context.Context, data *RuntimeData) error {
	if err := validateRuntimeData(data); err != nil {
		return err
	}
	if err := s.SyncRuntimeData(ctx, data); err != nil {
		return err
	}
	return s.store.SaveRuntimeData(ctx, data)
}

func validateRuntimeData(data *RuntimeData) error {
	if data == nil {
		return fmt.Errorf("runtime data is nil")
	}
	listenerIDs := make(map[int]struct{}, len(data.Listeners))
	for _, l := range data.Listeners {
		if l.ConfigurationId <= 0 {
			return fmt.Errorf("listener configurationId must be > 0")
		}
		if _, exists := listenerIDs[l.ConfigurationId]; exists {
			return fmt.Errorf("duplicate listener configurationId: %d", l.ConfigurationId)
		}
		listenerIDs[l.ConfigurationId] = struct{}{}
		if strings.TrimSpace(l.Address) == "" {
			return fmt.Errorf("listener address is empty (id=%d)", l.ConfigurationId)
		}
		if l.Port <= 0 || l.Port > 65535 {
			return fmt.Errorf("listener port is invalid (id=%d)", l.ConfigurationId)
		}
	}

	modemIDs := make(map[string]struct{}, len(data.Routing))
	for _, r := range data.Routing {
		id := strings.TrimSpace(r.ModemID)
		if id == "" {
			return fmt.Errorf("routing modemID is empty")
		}
		if _, exists := modemIDs[id]; exists {
			return fmt.Errorf("duplicate routing modemID: %s", id)
		}
		modemIDs[id] = struct{}{}
	}

	portIDs := make(map[int]struct{}, len(data.COMPorts))
	for _, p := range data.COMPorts {
		if p.PortID <= 0 {
			return fmt.Errorf("com port id must be > 0")
		}
		if _, exists := portIDs[p.PortID]; exists {
			return fmt.Errorf("duplicate com port id: %d", p.PortID)
		}
		portIDs[p.PortID] = struct{}{}
		if strings.TrimSpace(p.PortName) == "" {
			return fmt.Errorf("com port name is empty (id=%d)", p.PortID)
		}
	}

	for _, r := range data.COMRoutes {
		if _, exists := portIDs[r.PortID]; !exists {
			return fmt.Errorf("com route references unknown port id: %d", r.PortID)
		}
		if r.BeginRange != 0 && r.EndRange < r.BeginRange {
			return fmt.Errorf("com route range is invalid for site %d", r.SiteID)
		}
	}
	return nil
}

func (s *Server) handleModemConnection(ctx context.Context, conn net.Conn, lnCfg ListenerConfig) {
	expectedType := lnCfg.ModemType
	recvDeadline := time.Now().Add(s.cfg.RecvTimeout)
	_ = conn.SetReadDeadline(recvDeadline)
	_ = conn.SetWriteDeadline(time.Now().Add(s.cfg.SendTimeout))

	// Handshake: read 20 bytes modem identifier (ASCII, padded with '\0').
	idBuf := make([]byte, 20)
	n, err := io.ReadFull(conn, idBuf)
	if err != nil || n < 20 {
		log.Printf("handshake: failed read modem id from %v: %v", conn.RemoteAddr(), err)
		_ = conn.Close()
		return
	}

	// Validate handshake start byte (same intent as original C#).
	if (idBuf[0] == byte(10) && expectedType == ModemTypeLegacy) ||
		(idBuf[0] == byte(192) && expectedType == ModemTypeATSWP) {
		log.Printf("handshake: invalid modem id prefix from %v", conn.RemoteAddr())
		_ = conn.Close()
		return
	}

	modemID := normalizeNameVSBytes(idBuf)
	modemID = strings.TrimSpace(modemID)
	if modemID == "" {
		log.Printf("handshake: empty modemID from %v", conn.RemoteAddr())
		_ = conn.Close()
		return
	}

	// Close previous connection (if any).
	s.closeExistingModemConn(modemID)

	route := s.getOrCreateRouting(modemID)
	if route == nil {
		log.Printf("modem %q not routed; closing conn from %v", modemID, conn.RemoteAddr())
		_ = conn.Close()
		return
	}

	route.MbConnect = true
	_ = s.store.SaveConnectionEvent(ctx, modemID, route.SiteID, true, time.Now())

	// ACK: original server sends normalized listener.ServerID as 20 ASCII bytes.
	ack := normalizeNameFixedLen20(lnCfg.ServerID)
	_, _ = conn.Write(ack)

	c := &ModemConn{
		ModemID:   modemID,
		ModemType: expectedType,
		conn:      conn,
		writeCh:   make(chan []byte, 256),
		closeCh:   make(chan struct{}),
		lastRead:  time.Now(),
		lastWrite: time.Now(),
	}

	// Remember it.
	s.mu.Lock()
	s.conns[modemID] = c
	s.mu.Unlock()

	// Start reader and writer.
	go s.modemWriter(ctx, c)
	go s.modemReader(ctx, c)
}

func (s *Server) closeExistingModemConn(modemID string) {
	s.mu.Lock()
	defer s.mu.Unlock()
	if old, ok := s.conns[modemID]; ok {
		// Best effort.
		select {
		case <-old.closeCh:
		default:
			close(old.closeCh)
		}
		_ = old.conn.Close()
		delete(s.conns, modemID)
	}
}

func (s *Server) DisconnectModem(modemID string) bool {
	s.mu.RLock()
	_, ok := s.conns[modemID]
	s.mu.RUnlock()
	if !ok {
		return false
	}
	s.closeExistingModemConn(modemID)
	return true
}

func (s *Server) ConnectClient(machineName string) bool {
	name := strings.TrimSpace(machineName)
	if name == "" {
		return false
	}
	s.mu.Lock()
	defer s.mu.Unlock()
	if s.clientMachine == "" {
		s.clientMachine = name
		return true
	}
	return s.clientMachine == name
}

func (s *Server) DisconnectClient() {
	s.mu.Lock()
	s.clientMachine = ""
	s.mu.Unlock()
}

func (s *Server) IsClientAllowed(machineName string) bool {
	name := strings.TrimSpace(machineName)
	s.mu.RLock()
	owner := s.clientMachine
	s.mu.RUnlock()
	if owner == "" {
		return true
	}
	return strings.EqualFold(owner, name)
}

func (s *Server) Break(modemID string) bool {
	return s.DisconnectModem(modemID)
}

func (s *Server) DataSetView() map[string]any {
	return map[string]any{
		"runtime": s.CurrentRuntimeData(),
		"modems":  s.ModemStates(),
		"metrics": s.Metrics(),
	}
}

func (s *Server) getOrCreateRouting(modemID string) *Routing {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route != nil {
		return route
	}

	if !s.cfg.ConnectUnknown {
		return nil
	}

	siteID := -1
	if i, err := strconv.Atoi(modemID); err == nil {
		siteID = i
	}

	route = &Routing{
		SiteID:    siteID,
		ModemID:   modemID,
		MbConnect: true,
		Monitor:   NewMonitorQueue(s.cfg.QueueSize),
		Command:   NewMonitorQueue(s.cfg.QueueSize),
		Config:    NewMonitorQueue(s.cfg.QueueSize),
		CommState: NewMonitorQueue(s.cfg.QueueSize),
	}
	route.Command.SetInUse(false)
	route.Config.SetInUse(false)
	route.CommState.SetInUse(false)
	s.mu.Lock()
	s.routingByModem[modemID] = route
	s.mu.Unlock()
	return route
}

func (s *Server) modemReader(ctx context.Context, c *ModemConn) {
	defer func() {
		// Cleanup.
		s.mu.Lock()
		delete(s.conns, c.ModemID)
		if route := s.routingByModem[c.ModemID]; route != nil {
			route.MbConnect = false
			_ = s.store.SaveConnectionEvent(context.Background(), c.ModemID, route.SiteID, false, time.Now())
		}
		s.mu.Unlock()
		_ = c.conn.Close()
	}()

	recvBuf := make([]byte, 10000)
	for {
		select {
		case <-c.closeCh:
			return
		case <-ctx.Done():
			return
		default:
		}

		_ = c.conn.SetReadDeadline(time.Now().Add(s.cfg.RecvTimeout))
		n, err := c.conn.Read(recvBuf)
		if n <= 0 {
			// If nothing was read, handle errors normally.
			if err != nil {
				if ne, ok := err.(net.Error); ok && ne.Timeout() {
					continue
				}
				if errors.Is(err, io.EOF) {
					return
				}
				log.Printf("read error modem=%q: %v", c.ModemID, err)
				atomic.AddInt64(&s.readErrors, 1)
				return
			}
			continue
		}

		// Process the bytes even if Read() returned io.EOF together with n>0.
		if err != nil && errors.Is(err, io.EOF) {
			// We'll still parse; connection will be dropped after.
		} else if err != nil {
			if ne, ok := err.(net.Error); ok && ne.Timeout() {
				// Continue after parsing.
			} else {
				log.Printf("read error modem=%q: %v", c.ModemID, err)
				atomic.AddInt64(&s.readErrors, 1)
				return
			}
		}

		s.mu.RLock()
		route := s.routingByModem[c.ModemID]
		s.mu.RUnlock()
		if route == nil {
			return
		}

		c.inTraffic += int64(n)
		c.lastRead = time.Now()

		// Parse and push frames to monitor.
		if c.ModemType == ModemTypeLegacy {
			combined := make([]byte, len(c.legacyRemain)+n)
			copy(combined, c.legacyRemain)
			copy(combined[len(c.legacyRemain):], recvBuf[:n])

			remain, pstats := parseLegacyFramesDetailed(combined, SendToCOM, c.ModemID, c.ModemType, func(fr BatchRecord) {
				// Store only the parsed frame; direction is "received".
				if len(fr.Raw) == 0 {
					atomic.AddInt64(&s.droppedFrames, 1)
					return
				}
				route.Monitor.Push(fr)
				_ = s.store.SaveFrame(context.Background(), fr)
				_ = s.comManager.WriteByRoute(fr.SiteIDTo, fr.SiteIDFrom, fr.Raw)
			})

			c.legacyRemain = remain
			if pstats.Malformed > 0 {
				atomic.AddInt64(&s.parseErrors, int64(pstats.Malformed))
			}
		} else {
			// ATSWP: parse according to GetBatchsATSWP and feed content into legacy parser.
			combinedATSWP := make([]byte, len(c.atswpRemainATSWP)+n)
			copy(combinedATSWP, c.atswpRemainATSWP)
			copy(combinedATSWP[len(c.atswpRemainATSWP):], recvBuf[:n])

			frames, legacyRemain, atswpRemain, astats := parseATSWPFramesDetailed(
				combinedATSWP,
				SendToCOM,
				c.ModemID,
				c.ModemType,
				c.legacyRemain,
				nil,
				func(fr BatchRecord) {
					if len(fr.Raw) == 0 {
						atomic.AddInt64(&s.droppedFrames, 1)
						return
					}
					route.Monitor.Push(fr)
					if fr.LinkID == 196 {
						route.Command.Push(fr)
					}
					if fr.LinkID == 197 {
						route.Config.Push(fr)
					}
					if fr.LinkID == 198 {
						route.CommState.Push(fr)
					}
					_ = s.store.SaveFrame(context.Background(), fr)
					_ = s.comManager.WriteByRoute(fr.SiteIDTo, fr.SiteIDFrom, fr.Raw)
				},
			)
			_ = frames
			c.legacyRemain = legacyRemain
			c.atswpRemainATSWP = atswpRemain
			if astats.Malformed > 0 {
				atomic.AddInt64(&s.parseErrors, int64(astats.Malformed))
			}
		}

		if err != nil && errors.Is(err, io.EOF) {
			return
		}
	}
}

func (s *Server) modemWriter(ctx context.Context, c *ModemConn) {
	for {
		select {
		case <-c.closeCh:
			return
		case <-ctx.Done():
			return
		case payload := <-c.writeCh:
			if len(payload) == 0 {
				continue
			}
			_ = c.conn.SetWriteDeadline(time.Now().Add(s.cfg.SendTimeout))
			_, err := c.conn.Write(payload)
			if err != nil {
				log.Printf("write error modem=%q: %v", c.ModemID, err)
				atomic.AddInt64(&s.writeErrors, 1)
				return
			}
			c.lastWrite = time.Now()
		}
	}
}

func (s *Server) Metrics() ServerMetrics {
	s.mu.RLock()
	active := len(s.conns)
	known := len(s.routingByModem)
	s.mu.RUnlock()
	return ServerMetrics{
		UptimeSeconds:     int64(time.Since(s.startedAt).Seconds()),
		ActiveConnections: active,
		KnownModems:       known,
		ReadErrors:        atomic.LoadInt64(&s.readErrors),
		WriteErrors:       atomic.LoadInt64(&s.writeErrors),
		ParseErrors:       atomic.LoadInt64(&s.parseErrors),
		DroppedFrames:     atomic.LoadInt64(&s.droppedFrames),
		COM:               s.comManager.Metrics(),
	}
}

// EnqueueModemWrite sends raw bytes directly to a modem connection.
// (MVP helper, not part of original C# interface.)
func (s *Server) EnqueueModemWrite(modemID string, payload []byte) error {
	s.mu.RLock()
	c := s.conns[modemID]
	s.mu.RUnlock()
	if c == nil {
		return fmt.Errorf("modem %q not connected", modemID)
	}
	select {
	case c.writeCh <- bytesCopy(payload):
		c.outTraffic += int64(len(payload))
		return nil
	case <-time.After(500 * time.Millisecond):
		return fmt.Errorf("modem %q write queue timeout", modemID)
	case <-c.closeCh:
		return fmt.Errorf("modem %q closing", modemID)
	}
}

func (s *Server) SendToModem(modemID string, payload []byte, wrapATSWP bool, batchType byte) error {
	s.mu.RLock()
	c := s.conns[modemID]
	s.mu.RUnlock()
	if c == nil {
		return fmt.Errorf("modem %q not connected", modemID)
	}

	data := payload
	if wrapATSWP || c.ModemType == ModemTypeATSWP {
		data = wrapATSWPBatch(payload, batchType)
	}
	return s.EnqueueModemWrite(modemID, data)
}

func (s *Server) ModemStates() []ModemState {
	s.mu.RLock()
	defer s.mu.RUnlock()

	states := make([]ModemState, 0, len(s.routingByModem))
	for modemID, route := range s.routingByModem {
		st := ModemState{
			ModemID:   modemID,
			SiteID:    route.SiteID,
			Connected: route.MbConnect,
			QueueDepth: route.Monitor.Len(),
			CommandQueueDepth: route.Command.Len(),
			ConfigQueueDepth: route.Config.Len(),
			CommStateQueueDepth: route.CommState.Len(),
		}
		if c := s.conns[modemID]; c != nil {
			st.ModemType = c.ModemType.String()
			st.InTraffic = c.inTraffic
			st.OutTraffic = c.outTraffic
		} else {
			st.ModemType = ModemTypeLegacy.String()
		}
		states = append(states, st)
	}
	return states
}

func (s *Server) SetUseCommand(modemID string, use bool) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	route.Command.SetInUse(use)
	return true
}

func (s *Server) GetUseCommand(modemID string) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	return route.Command.InUse()
}

func (s *Server) GetCommandData(modemID string, clear bool) []BatchRecord {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return nil
	}
	return route.Command.Snapshot(clear)
}

func (s *Server) SetUseConfig(modemID string, use bool) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	route.Config.SetInUse(use)
	return true
}

func (s *Server) GetUseConfig(modemID string) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	return route.Config.InUse()
}

func (s *Server) GetConfigData(modemID string, clear bool) []BatchRecord {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return nil
	}
	return route.Config.Snapshot(clear)
}

func (s *Server) SetUseCommState(modemID string, use bool) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	route.CommState.SetInUse(use)
	return true
}

func (s *Server) GetUseCommState(modemID string) bool {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return false
	}
	return route.CommState.InUse()
}

func (s *Server) GetCommStateData(modemID string, clear bool) []BatchRecord {
	s.mu.RLock()
	route := s.routingByModem[modemID]
	s.mu.RUnlock()
	if route == nil {
		return nil
	}
	return route.CommState.Snapshot(clear)
}

func (s *Server) SendToSite(siteID int, payload []byte) error {
	s.mu.RLock()
	defer s.mu.RUnlock()
	for modemID, route := range s.routingByModem {
		if route.SiteID != siteID {
			continue
		}
		c := s.conns[modemID]
		if c == nil {
			return fmt.Errorf("site %d modem %s not connected", siteID, modemID)
		}
		select {
		case c.writeCh <- bytesCopy(payload):
			c.outTraffic += int64(len(payload))
			return nil
		case <-time.After(500 * time.Millisecond):
			return fmt.Errorf("modem %q write queue timeout", modemID)
		case <-c.closeCh:
			return fmt.Errorf("modem %q closing", modemID)
		}
	}
	return fmt.Errorf("site %d routing not found", siteID)
}

func (s *Server) StartConnectionWatchdog(ctx context.Context) {
	ticker := time.NewTicker(10 * time.Second)
	go func() {
		defer ticker.Stop()
		for {
			select {
			case <-ctx.Done():
				return
			case now := <-ticker.C:
				idleCutoff := now.Add(-time.Duration(s.cfg.CleanupTimeS) * time.Second)
				var toClose []string
				s.mu.RLock()
				for modemID, c := range s.conns {
					if c.lastRead.Before(idleCutoff) && c.lastWrite.Before(idleCutoff) {
						toClose = append(toClose, modemID)
					}
				}
				s.mu.RUnlock()
				for _, modemID := range toClose {
					s.closeExistingModemConn(modemID)
				}
			}
		}
	}()
}

func (s *Server) StartStatisticsWorker(ctx context.Context) {
	interval := time.Duration(s.cfg.StatisticsIntervalSeconds) * time.Second
	if interval <= 0 {
		interval = time.Hour
	}
	ticker := time.NewTicker(interval)
	go func() {
		defer ticker.Stop()
		for {
			select {
			case <-ctx.Done():
				return
			case now := <-ticker.C:
				states := s.ModemStates()
				if err := s.store.SaveStatisticsSnapshot(ctx, states, now); err != nil {
					log.Printf("save stats snapshot failed: %v", err)
				}
				cutoff := now.Add(-time.Duration(s.cfg.StatisticsRetentionHours) * time.Hour)
				if err := s.store.PruneOldData(ctx, cutoff); err != nil {
					log.Printf("prune old data failed: %v", err)
				}
			}
		}
	}()
}

