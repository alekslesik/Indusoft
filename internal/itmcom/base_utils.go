package itmcom

import (
	"fmt"
	"net"
	"strings"
)

// GetStringCommandTypeCompat maps legacy command direction enum to text token.
func GetStringCommandTypeCompat(t StringCommandType) string {
	switch t {
	case StringCommandTypeRequest:
		return "request"
	case StringCommandTypeAnswer:
		return "answer"
	default:
		return "error"
	}
}

// TranslateModemBatchCompat renders bytes as uppercase hex with trailing spaces.
func TranslateModemBatchCompat(data []byte, begin, size int) string {
	if size <= 0 || begin < 0 || begin >= len(data) {
		return ""
	}
	end := begin + size
	if end > len(data) {
		end = len(data)
	}
	return hexByteString(data[begin:end])
}

// NormalizeNameCompat returns fixed-width 20-byte legacy modem identifier payload.
func NormalizeNameCompat(name string) []byte {
	return normalizeNameFixedLen20(name)
}

// TCPIPClientCompat mirrors legacy TCPIPClient as a simple connection holder.
type TCPIPClientCompat struct {
	Point  *net.TCPAddr
	Conn   *net.TCPConn
	SiteID int
}

// NewTCPIPClientCompat creates a compatibility TCPIP client descriptor.
func NewTCPIPClientCompat(siteID int) *TCPIPClientCompat {
	return &TCPIPClientCompat{SiteID: siteID}
}

// ParsePoint parses legacy "host:port" endpoint key.
func (c *TCPIPClientCompat) ParsePoint(key string) error {
	host, port, err := net.SplitHostPort(strings.TrimSpace(key))
	if err != nil {
		return err
	}
	addr, err := net.ResolveTCPAddr("tcp", net.JoinHostPort(host, port))
	if err != nil {
		return err
	}
	c.Point = addr
	return nil
}

// Connect dials parsed endpoint and applies legacy buffer sizing defaults.
func (c *TCPIPClientCompat) Connect() error {
	if c.Point == nil {
		return fmt.Errorf("tcp endpoint is not set")
	}
	conn, err := net.DialTCP("tcp", nil, c.Point)
	if err != nil {
		return err
	}
	_ = conn.SetLinger(10)
	_ = conn.SetReadBuffer(16 * 1024)
	_ = conn.SetWriteBuffer(16 * 1024)
	c.Conn = conn
	return nil
}
