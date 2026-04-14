package itmcom

import "log"

// EventLoggerCompat provides legacy-style static logging entrypoints.
type EventLoggerCompat struct{}

// Info writes informational compatibility logs.
func (EventLoggerCompat) Info(msg string) {
	log.Printf("INFO: %s", msg)
}

// Error writes error compatibility logs.
func (EventLoggerCompat) Error(msg string) {
	log.Printf("ERROR: %s", msg)
}

// Logger is a package-level compatibility logger instance.
var Logger EventLoggerCompat
