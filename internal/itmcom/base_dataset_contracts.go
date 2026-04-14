package itmcom

// ITMCOMDataSetCompat mirrors legacy ITMCOMDataSet table names in a Go-friendly shape.
// Typed fields are preferred where the runtime already has stable contracts.
type ITMCOMDataSetCompat struct {
	Runtime RuntimeData    `json:"runtime"`
	Modems  []ModemState   `json:"modems"`
	Metrics ServerMetrics  `json:"metrics"`
	Tables  map[string]any `json:"tables"`
}

var legacyDataSetTableNames = []string{
	"Config",
	"Dynamic",
	"Static",
	"Log",
	"Routing",
	"LogStatus",
	"Port",
	"Modem",
	"IllegalModem",
	"ConfigType",
	"TCPIP",
	"DynamicTCPIP",
	"TCPIPClient",
	"DynamicTCPIPClient",
}

// BuildITMCOMDataSetCompat builds a compatibility dataset projection from current server state.
func BuildITMCOMDataSetCompat(s *Server) ITMCOMDataSetCompat {
	rdPtr := s.CurrentRuntimeData()
	rd := RuntimeData{}
	if rdPtr != nil {
		rd = *rdPtr
	}
	states := s.ModemStates()
	metrics := s.Metrics()

	tables := make(map[string]any, len(legacyDataSetTableNames))
	for _, name := range legacyDataSetTableNames {
		tables[name] = []map[string]any{}
	}
	tables["Config"] = rd.Listeners
	tables["Routing"] = rd.COMRoutes
	tables["Port"] = rd.COMPorts
	tables["Modem"] = states

	return ITMCOMDataSetCompat{
		Runtime: rd,
		Modems:  states,
		Metrics: metrics,
		Tables:  tables,
	}
}

// AnComBaseCompatCore captures legacy IAnComBase operations already implemented in Server.
type AnComBaseCompatCore interface {
	ConnectClient(machineName string) bool
	DisconnectClient()
	Break(modemID string) bool
	DataSetView() map[string]any
	GetUseCommand(modemID string) bool
	SetUseCommand(modemID string, use bool) bool
	GetCommandData(modemID string, clear bool) []BatchRecord
	GetUseConfig(modemID string) bool
	SetUseConfig(modemID string, use bool) bool
	GetConfigData(modemID string, clear bool) []BatchRecord
	GetUseCommState(modemID string) bool
	SetUseCommState(modemID string, use bool) bool
	GetCommStateData(modemID string, clear bool) []BatchRecord
	SendToModem(modemID string, payload []byte, wrapATSWP bool, batchType byte) error
}

// Compile-time guard: Server must keep implementing the mapped IAnComBase core contract.
var _ AnComBaseCompatCore = (*Server)(nil)
