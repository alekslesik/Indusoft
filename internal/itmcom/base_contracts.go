package itmcom

import (
	"fmt"
	"time"
)

// ConfigType mirrors legacy Indusoft Base configuration transport kinds.
type ConfigType int

const (
	ConfigTypeModem        ConfigType = 1
	ConfigTypeTCPIPStatic  ConfigType = 2
	ConfigTypeTCPIPDynamic ConfigType = 3
	ConfigTypeModemATSWP   ConfigType = 4
	ConfigTypeTCPIPClient  ConfigType = 5
)

// Direction mirrors legacy send/receive direction semantics.
type Direction int

const (
	DirectionSend Direction = iota
	DirectionReceive
)

// StringCommandType mirrors legacy command text buckets.
type StringCommandType int

const (
	StringCommandTypeRequest StringCommandType = iota
	StringCommandTypeAnswer
	StringCommandTypeError
)

// ModemUpdateFlag mirrors legacy modem-state update categories.
type ModemUpdateFlag int

const (
	ModemUpdateFlagNone ModemUpdateFlag = iota
	ModemUpdateFlagConnection
	ModemUpdateFlagRelcase
	ModemUpdateFlagLastBatch
)

// ATSWPBatchType mirrors ATSWP link IDs used by legacy service.
type ATSWPBatchType byte

const (
	ATSWPBatchTypeDataUART1 ATSWPBatchType = 0
	ATSWPBatchTypeDataUART2 ATSWPBatchType = 1
	ATSWPBatchTypeDataI2C   ATSWPBatchType = 2
	ATSWPBatchTypeDataSPI1  ATSWPBatchType = 3
	ATSWPBatchTypeDataSPI2  ATSWPBatchType = 4
	ATSWPBatchTypeDataUSB   ATSWPBatchType = 5

	ATSWPBatchTypeCommand            ATSWPBatchType = 196
	ATSWPBatchTypeConfig             ATSWPBatchType = 197
	ATSWPBatchTypeCommunicationState ATSWPBatchType = 198
)

const (
	batchDirectionCommand       = "command"
	batchDirectionCommandAnswer = "command-answer"
	batchDirectionNoReceiver    = "no-receiver"
)

// BatchDataCompat keeps legacy display-ready batch fields as strings.
type BatchDataCompat struct {
	UnitSiteID string `json:"unitSiteId"`
	Direction  string `json:"direction"`
	Time       string `json:"time"`
	SiteID     string `json:"siteId"`
	Batch      string `json:"batch"`
}

// NewBatchDataCompatFromDirection builds BatchDataCompat using send/receive semantics.
func NewBatchDataCompatFromDirection(unitSiteID int, direction Direction, at time.Time, siteID int, batch string) BatchDataCompat {
	out := BatchDataCompat{
		UnitSiteID: fmt.Sprintf("%d", unitSiteID),
		Time:       formatCompatTime(at),
		SiteID:     fmt.Sprintf("%d", siteID),
		Batch:      batch,
	}
	if direction == DirectionSend {
		out.Direction = fmt.Sprintf("send %d", siteID)
		return out
	}
	switch siteID {
	case -3:
		out.Direction = batchDirectionCommandAnswer
	case -2:
		out.Direction = batchDirectionCommand
	case -1:
		out.Direction = batchDirectionNoReceiver
	default:
		out.Direction = fmt.Sprintf("receive %d", siteID)
	}
	return out
}

// NewBatchDataCompatFromCommand builds BatchDataCompat from command request/answer/error direction.
func NewBatchDataCompatFromCommand(unitSiteID int, direction StringCommandType, at time.Time, siteID int, batch string) BatchDataCompat {
	out := BatchDataCompat{
		UnitSiteID: fmt.Sprintf("%d", unitSiteID),
		Time:       formatCompatTime(at),
		SiteID:     fmt.Sprintf("%d", siteID),
		Batch:      batch,
	}
	switch direction {
	case StringCommandTypeRequest:
		out.Direction = "request"
	case StringCommandTypeAnswer:
		out.Direction = "answer"
	default:
		out.Direction = "error"
	}
	return out
}

// NewLogsCompat mirrors legacy NewLogs DTO.
type NewLogsCompat struct {
	LogID    int       `json:"logId"`
	SiteID   int       `json:"siteId"`
	COMPort  string    `json:"comPort"`
	Time     time.Time `json:"time"`
	Message  string    `json:"message"`
	StatusID int       `json:"statusId"`
}

// NewModemsCompat mirrors legacy NewModems DTO.
type NewModemsCompat struct {
	ID      int    `json:"id"`
	SiteID  int    `json:"siteId"`
	ModemID string `json:"modemId"`
	MbAdd   bool   `json:"mbAdd"`
}

// NewUpdatedModemsCompat mirrors legacy UpdatedModems DTO.
type NewUpdatedModemsCompat struct {
	ID        int             `json:"id"`
	MbConnect bool            `json:"mbConnect"`
	Time      time.Time       `json:"time"`
	Flag      ModemUpdateFlag `json:"flag"`
}

func formatCompatTime(at time.Time) string {
	base := at.Format("2006-01-02 15:04:05")
	return fmt.Sprintf("%s,%03d", base, at.Nanosecond()/1_000_000)
}
