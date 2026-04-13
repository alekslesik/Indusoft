package itmcom

import (
	"encoding/json"
	"fmt"
	"os"
	"strings"
	"time"
)

type ModemType int

const (
	ModemTypeLegacy ModemType = 0
	ModemTypeATSWP  ModemType = 2
)

func (m ModemType) String() string {
	switch m {
	case ModemTypeLegacy:
		return "Legacy"
	case ModemTypeATSWP:
		return "ATSWP"
	default:
		return fmt.Sprintf("ModemType(%d)", int(m))
	}
}

func parseModemType(s string) (ModemType, error) {
	switch strings.ToLower(strings.TrimSpace(s)) {
	case "legacy":
		return ModemTypeLegacy, nil
	case "atswp":
		return ModemTypeATSWP, nil
	default:
		return 0, fmt.Errorf("unknown modemType %q", s)
	}
}

type ListenerConfig struct {
	// ConfigurationId matches the original Remoting configurationId concept.
	ConfigurationId int `json:"configurationId"`

	Address  string    `json:"address"`
	Port      int       `json:"port"`
	ServerID  string    `json:"serverID"`
	ModemType ModemType `json:"modemType"`
}

type RoutingConfig struct {
	SiteID int    `json:"siteID"`
	ModemID string `json:"modemID"`
}

type COMPortConfig struct {
	PortID   int    `json:"portId"`
	PortName string `json:"portName"`
	BaudRate int    `json:"baudRate"`
	DataBits int    `json:"dataBits"`
	StopBits int    `json:"stopBits"`
	Parity   string `json:"parity"`
}

type COMRouteConfig struct {
	SiteID      int `json:"siteID"`
	PortID      int `json:"portId"`
	BeginRange  int `json:"beginRange"`
	EndRange    int `json:"endRange"`
	LocalSiteID int `json:"localSiteID"`
}

type ServerConfig struct {
	RecvTimeout  time.Duration `json:"-"`
	SendTimeout  time.Duration `json:"-"`
	QueueSize    int           `json:"queueSize"`
	CleanupTimeS int           `json:"cleanupTimeSeconds"`

	// If false: unknown modemID is rejected immediately.
	ConnectUnknown bool `json:"connectUnknown"`

	HandshakeTimeoutSeconds int `json:"handshakeTimeoutSeconds"`

	Listeners []ListenerConfig `json:"listeners"`
	Routing   []RoutingConfig `json:"routing"`
	COMPorts  []COMPortConfig  `json:"comPorts"`
	COMRoutes []COMRouteConfig `json:"comRoutes"`

	// Optional listen backlog.
	ListenBacklog int `json:"listenBacklog"`

	// Timeouts in milliseconds (used to build RecvTimeout/SendTimeout).
	RecvTimeoutMs int `json:"recvTimeoutMs"`
	SendTimeoutMs int `json:"sendTimeoutMs"`

	HTTPAddr string `json:"httpAddr"`
	HTTPPort int    `json:"httpPort"`

	DBConnString string `json:"dbConnString"`
	DBAutoMigrate bool  `json:"dbAutoMigrate"`
	DBUseStoredProcedures bool `json:"dbUseStoredProcedures"`

	StatisticsIntervalSeconds int `json:"statisticsIntervalSeconds"`
	StatisticsRetentionHours  int `json:"statisticsRetentionHours"`

	APIToken            string `json:"apiToken"`
	APIRateLimitPerMin  int    `json:"apiRateLimitPerMinute"`
}

type configFile struct {
	QueueSize    int              `json:"queueSize"`
	CleanupTimeS int              `json:"cleanupTimeSeconds"`
	ConnectUnknown bool          `json:"connectUnknown"`
	HandshakeTimeoutSeconds int  `json:"handshakeTimeoutSeconds"`
	RecvTimeoutMs int            `json:"recvTimeoutMs"`
	SendTimeoutMs int            `json:"sendTimeoutMs"`
	HTTPAddr string `json:"httpAddr"`
	HTTPPort int    `json:"httpPort"`
	DBConnString string `json:"dbConnString"`
	DBAutoMigrate bool  `json:"dbAutoMigrate"`
	DBUseStoredProcedures bool `json:"dbUseStoredProcedures"`
	StatisticsIntervalSeconds int `json:"statisticsIntervalSeconds"`
	StatisticsRetentionHours  int `json:"statisticsRetentionHours"`
	APIToken            string `json:"apiToken"`
	APIRateLimitPerMin  int    `json:"apiRateLimitPerMinute"`

	ListenBacklog int `json:"listenBacklog"`

	Listeners []struct {
		ConfigurationId int    `json:"configurationId"`
		Address         string `json:"address"`
		Port            int    `json:"port"`
		ServerID        string `json:"serverID"`
		ModemType       string `json:"modemType"`
	} `json:"listeners"`
	Routing []RoutingConfig `json:"routing"`
	COMPorts  []COMPortConfig  `json:"comPorts"`
	COMRoutes []COMRouteConfig `json:"comRoutes"`
}

func LoadConfig(path string) (*ServerConfig, error) {
	b, err := os.ReadFile(path)
	if err != nil {
		return nil, fmt.Errorf("read config %q: %w", path, err)
	}

	var cf configFile
	if err := json.Unmarshal(b, &cf); err != nil {
		return nil, fmt.Errorf("parse config %q: %w", path, err)
	}

	cfg := &ServerConfig{
		QueueSize:                 cf.QueueSize,
		CleanupTimeS:              cf.CleanupTimeS,
		ConnectUnknown:           cf.ConnectUnknown,
		HandshakeTimeoutSeconds: cf.HandshakeTimeoutSeconds,
		RecvTimeoutMs:            cf.RecvTimeoutMs,
		SendTimeoutMs:            cf.SendTimeoutMs,
		ListenBacklog:           cf.ListenBacklog,
		Routing:                  cf.Routing,
		COMPorts:                 cf.COMPorts,
		COMRoutes:                cf.COMRoutes,
		DBConnString:             strings.TrimSpace(cf.DBConnString),
		DBAutoMigrate:            cf.DBAutoMigrate,
		DBUseStoredProcedures:    cf.DBUseStoredProcedures,
		StatisticsIntervalSeconds: cf.StatisticsIntervalSeconds,
		StatisticsRetentionHours:  cf.StatisticsRetentionHours,
		APIToken:                 strings.TrimSpace(cf.APIToken),
		APIRateLimitPerMin:       cf.APIRateLimitPerMin,
	}

	// Defaults.
	if cfg.QueueSize <= 0 {
		cfg.QueueSize = 10000
	}
	if cfg.CleanupTimeS <= 0 {
		cfg.CleanupTimeS = 60
	}
	if cfg.HandshakeTimeoutSeconds <= 0 {
		cfg.HandshakeTimeoutSeconds = 10
	}
	if cfg.RecvTimeoutMs <= 0 {
		cfg.RecvTimeoutMs = 15000
	}
	if cfg.SendTimeoutMs <= 0 {
		cfg.SendTimeoutMs = 15000
	}
	if cfg.StatisticsIntervalSeconds <= 0 {
		cfg.StatisticsIntervalSeconds = 3600
	}
	if cfg.StatisticsRetentionHours <= 0 {
		cfg.StatisticsRetentionHours = 2880
	}
	if cfg.APIRateLimitPerMin <= 0 {
		cfg.APIRateLimitPerMin = 300
	}

	cfg.RecvTimeout = time.Duration(cfg.RecvTimeoutMs) * time.Millisecond
	cfg.SendTimeout = time.Duration(cfg.SendTimeoutMs) * time.Millisecond
	cfg.HTTPAddr = strings.TrimSpace(cf.HTTPAddr)
	cfg.HTTPPort = cf.HTTPPort
	if cfg.HTTPAddr == "" {
		cfg.HTTPAddr = "0.0.0.0"
	}
	if cfg.HTTPPort == 0 {
		cfg.HTTPPort = 8080
	}

	for _, l := range cf.Listeners {
		mt, err := parseModemType(l.ModemType)
		if err != nil {
			return nil, err
		}
		cfg.Listeners = append(cfg.Listeners, ListenerConfig{
			ConfigurationId: l.ConfigurationId,
			Address:         l.Address,
			Port:            l.Port,
			ServerID:        l.ServerID,
			ModemType:       mt,
		})
	}
	if len(cfg.Listeners) == 0 {
		return nil, fmt.Errorf("config must include at least one listener")
	}

	return cfg, nil
}

