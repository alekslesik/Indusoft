package itmcom

import (
	"net"
	"time"
)

type SendTo int

const (
	SendToCOM   SendTo = 0
	SendToModem SendTo = 1
)

// BatchRecord represents a parsed frame (legacy) or decoded payload.
type BatchRecord struct {
	Timestamp time.Time

	ModemID   string
	ModemType ModemType

	SiteIDFrom int
	SiteIDTo   int
	LinkID     byte
	BatchHex   string
	Raw        []byte
}

type Routing struct {
	SiteID   int
	ModemID  string
	MbConnect bool

	Monitor *MonitorQueue
	Command *MonitorQueue
	Config  *MonitorQueue
	CommState *MonitorQueue
}

type ModemConn struct {
	ModemID   string
	ModemType ModemType

	conn    net.Conn
	writeCh chan []byte

	closeCh chan struct{}

	// Legacy parsing state.
	legacyRemain []byte

	// ATSWP parsing state.
	atswpRemainATSWP []byte
	atswpRemainSize  int

	// Stats.
	inTraffic  int64
	outTraffic int64
	lastRead   time.Time
	lastWrite  time.Time
}

type ModemState struct {
	ModemID    string `json:"modemID"`
	ModemType  string `json:"modemType"`
	Connected  bool   `json:"connected"`
	SiteID     int    `json:"siteID"`
	InTraffic  int64  `json:"inTraffic"`
	OutTraffic int64  `json:"outTraffic"`
	QueueDepth int    `json:"queueDepth"`
	CommandQueueDepth int `json:"commandQueueDepth"`
	ConfigQueueDepth int `json:"configQueueDepth"`
	CommStateQueueDepth int `json:"commStateQueueDepth"`
}

type ServerMetrics struct {
	UptimeSeconds    int64 `json:"uptimeSeconds"`
	ActiveConnections int  `json:"activeConnections"`
	KnownModems       int  `json:"knownModems"`
	ReadErrors       int64 `json:"readErrors"`
	WriteErrors      int64 `json:"writeErrors"`
	ParseErrors      int64 `json:"parseErrors"`
	DroppedFrames    int64 `json:"droppedFrames"`
	COM              COMMetrics `json:"com"`
}

