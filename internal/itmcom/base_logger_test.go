package itmcom

import "testing"

func TestEventLoggerCompatMethods(t *testing.T) {
	Logger.Info("phase1 info")
	Logger.Error("phase1 error")
}

