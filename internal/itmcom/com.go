package itmcom

import (
	"context"
	"log"
	"strings"
	"sync"
	"sync/atomic"
	"time"

	"github.com/tarm/serial"
)

type comPort struct {
	cfg    COMPortConfig
	serialCfg *serial.Config
	handle *serial.Port
	mu     sync.Mutex

	remain []byte
	closed bool
}

type COMManager struct {
	mu sync.RWMutex

	ports  map[int]*comPort
	routes []COMRouteConfig

	openErrors int64
	readErrors int64
	writeErrors int64
	reconnects int64
}

func NewCOMManager() *COMManager {
	return &COMManager{
		ports: make(map[int]*comPort),
	}
}

func (m *COMManager) Sync(ctx context.Context, ports []COMPortConfig, routes []COMRouteConfig, onFrame func(BatchRecord)) {
	target := make(map[int]COMPortConfig, len(ports))
	for _, p := range ports {
		target[p.PortID] = p
	}

	m.mu.Lock()
	for id, cp := range m.ports {
		newCfg, ok := target[id]
		if !ok || !sameCOMPortConfig(cp.cfg, newCfg) {
			cp.closed = true
			if cp.handle != nil {
				_ = cp.handle.Close()
			}
			delete(m.ports, id)
		}
	}
	m.mu.Unlock()

	for _, cfg := range ports {
		m.mu.RLock()
		_, exists := m.ports[cfg.PortID]
		m.mu.RUnlock()
		if exists {
			continue
		}
		if err := m.startPort(ctx, cfg, onFrame); err != nil {
			log.Printf("com port start failed (%s): %v", cfg.PortName, err)
		}
	}

	m.mu.Lock()
	m.routes = routes
	m.mu.Unlock()
}

func (m *COMManager) startPort(ctx context.Context, cfg COMPortConfig, onFrame func(BatchRecord)) error {
	sc := &serial.Config{
		Name:        cfg.PortName,
		Baud:        cfg.BaudRate,
		ReadTimeout: 300 * time.Millisecond,
	}
	if cfg.DataBits > 0 {
		sc.Size = byte(cfg.DataBits)
	}
	switch strings.ToLower(strings.TrimSpace(cfg.Parity)) {
	case "odd", "1":
		sc.Parity = serial.ParityOdd
	case "even", "2":
		sc.Parity = serial.ParityEven
	default:
		sc.Parity = serial.ParityNone
	}
	switch cfg.StopBits {
	case 2:
		sc.StopBits = serial.Stop2
	default:
		sc.StopBits = serial.Stop1
	}

	p, err := serial.OpenPort(sc)
	if err != nil {
		atomic.AddInt64(&m.openErrors, 1)
		return err
	}
	cp := &comPort{cfg: cfg, handle: p, serialCfg: sc}
	m.mu.Lock()
	m.ports[cfg.PortID] = cp
	m.mu.Unlock()

	go m.readLoop(ctx, cp, onFrame)
	return nil
}

func (m *COMManager) readLoop(ctx context.Context, cp *comPort, onFrame func(BatchRecord)) {
	buf := make([]byte, 4096)
	for {
		select {
		case <-ctx.Done():
			return
		default:
		}
		n, err := cp.handle.Read(buf)
		if err != nil {
			atomic.AddInt64(&m.readErrors, 1)
			log.Printf("com read error (%s): %v", cp.cfg.PortName, err)
			if !m.reopenPort(cp) {
				return
			}
			continue
		}
		if n == 0 {
			continue
		}
		cp.mu.Lock()
		combined := make([]byte, len(cp.remain)+n)
		copy(combined, cp.remain)
		copy(combined[len(cp.remain):], buf[:n])
		cp.remain = parseLegacyFrames(combined, SendToModem, cp.cfg.PortName, ModemTypeLegacy, onFrame)
		cp.mu.Unlock()
	}
}

func (m *COMManager) WriteByRoute(siteIDTo, siteIDFrom int, payload []byte) bool {
	m.mu.RLock()
	defer m.mu.RUnlock()
	for _, r := range m.routes {
		if r.SiteID != siteIDTo {
			continue
		}
		if r.BeginRange != 0 && (siteIDFrom < r.BeginRange || siteIDFrom > r.EndRange) {
			continue
		}
		if cp := m.ports[r.PortID]; cp != nil {
			if cp.handle == nil {
				_ = m.reopenPort(cp)
				if cp.handle == nil {
					continue
				}
			}
			cp.mu.Lock()
			_, err := cp.handle.Write(payload)
			cp.mu.Unlock()
			if err == nil {
				return true
			}
			atomic.AddInt64(&m.writeErrors, 1)
			_ = m.reopenPort(cp)
		}
	}
	return false
}

func (m *COMManager) reopenPort(cp *comPort) bool {
	cp.mu.Lock()
	defer cp.mu.Unlock()
	if cp.closed {
		return false
	}
	if cp.handle != nil {
		_ = cp.handle.Close()
	}
	// bounded retries with linear backoff
	for i := 1; i <= 5; i++ {
		p, err := serial.OpenPort(cp.serialCfg)
		if err == nil {
			cp.handle = p
			atomic.AddInt64(&m.reconnects, 1)
			return true
		}
		atomic.AddInt64(&m.openErrors, 1)
		time.Sleep(time.Duration(i) * 200 * time.Millisecond)
	}
	return false
}

type COMMetrics struct {
	ActivePorts int   `json:"activePorts"`
	OpenErrors  int64 `json:"openErrors"`
	ReadErrors  int64 `json:"readErrors"`
	WriteErrors int64 `json:"writeErrors"`
	Reconnects  int64 `json:"reconnects"`
}

func (m *COMManager) Metrics() COMMetrics {
	m.mu.RLock()
	active := len(m.ports)
	m.mu.RUnlock()
	return COMMetrics{
		ActivePorts: active,
		OpenErrors:  atomic.LoadInt64(&m.openErrors),
		ReadErrors:  atomic.LoadInt64(&m.readErrors),
		WriteErrors: atomic.LoadInt64(&m.writeErrors),
		Reconnects:  atomic.LoadInt64(&m.reconnects),
	}
}

func sameCOMPortConfig(a, b COMPortConfig) bool {
	return a.PortID == b.PortID &&
		a.PortName == b.PortName &&
		a.BaudRate == b.BaudRate &&
		a.DataBits == b.DataBits &&
		a.StopBits == b.StopBits &&
		strings.EqualFold(strings.TrimSpace(a.Parity), strings.TrimSpace(b.Parity))
}

