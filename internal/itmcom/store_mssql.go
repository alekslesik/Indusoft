package itmcom

import (
	"context"
	"database/sql"
	"fmt"
	"strconv"
	"strings"
	"time"

	_ "github.com/microsoft/go-mssqldb"
)

type mssqlStore struct {
	db *sql.DB
	useStoredProcedures bool
}

const (
	legacyLogStatusInfo = 1
	legacyLogRecordsCnt = 500
)

func NewMSSQLStore(connString string, autoMigrate bool, useStoredProcedures bool) (Store, error) {
	db, err := sql.Open("sqlserver", connString)
	if err != nil {
		return nil, err
	}
	if err := db.Ping(); err != nil {
		_ = db.Close()
		return nil, err
	}
	s := &mssqlStore{db: db, useStoredProcedures: useStoredProcedures}
	if autoMigrate {
		if err := s.migrate(context.Background()); err != nil {
			_ = db.Close()
			return nil, err
		}
	}
	return s, nil
}

func (s *mssqlStore) Close() error { return s.db.Close() }

func (s *mssqlStore) LoadRuntimeData(ctx context.Context) (*RuntimeData, error) {
	rd := &RuntimeData{}
	if err := s.loadListeners(ctx, rd); err != nil {
		return nil, err
	}
	_ = s.loadRouting(ctx, rd)
	_ = s.loadCOMPorts(ctx, rd)
	_ = s.loadCOMRoutes(ctx, rd)
	return rd, nil
}

func (s *mssqlStore) SaveRuntimeData(ctx context.Context, data *RuntimeData) error {
	if data == nil {
		return nil
	}
	tx, err := s.db.BeginTx(ctx, nil)
	if err != nil {
		return err
	}
	defer func() { _ = tx.Rollback() }()

	_, _ = tx.ExecContext(ctx, `DELETE FROM ITMCOM_Listeners`)
	_, _ = tx.ExecContext(ctx, `DELETE FROM ITMCOM_Routing`)
	_, _ = tx.ExecContext(ctx, `DELETE FROM ITMCOM_COMRoutes`)
	_, _ = tx.ExecContext(ctx, `DELETE FROM ITMCOM_COMPorts`)

	for _, l := range data.Listeners {
		if _, err := tx.ExecContext(ctx, `
INSERT INTO ITMCOM_Listeners (ConfigurationId, Address, Port, ServerID, ModemType, IsCurrent)
VALUES (@p1, @p2, @p3, @p4, @p5, 1)`,
			l.ConfigurationId, l.Address, l.Port, l.ServerID, l.ModemType.String()); err != nil {
			return err
		}
	}
	for _, r := range data.Routing {
		if _, err := tx.ExecContext(ctx, `
INSERT INTO ITMCOM_Routing (SiteID, ModemID) VALUES (@p1, @p2)`,
			r.SiteID, r.ModemID); err != nil {
			return err
		}
	}
	for _, p := range data.COMPorts {
		if _, err := tx.ExecContext(ctx, `
INSERT INTO ITMCOM_COMPorts (PortID, PortName, BaudRate, DataBits, StopBits, Parity)
VALUES (@p1, @p2, @p3, @p4, @p5, @p6)`,
			p.PortID, p.PortName, p.BaudRate, p.DataBits, p.StopBits, p.Parity); err != nil {
			return err
		}
	}
	for _, r := range data.COMRoutes {
		if _, err := tx.ExecContext(ctx, `
INSERT INTO ITMCOM_COMRoutes (SiteID, PortID, BeginRange, EndRange, LocalSiteID)
VALUES (@p1, @p2, @p3, @p4, @p5)`,
			r.SiteID, r.PortID, r.BeginRange, r.EndRange, r.LocalSiteID); err != nil {
			return err
		}
	}

	// Optional legacy-schema synchronization for compatibility with stored-procedure ecosystem.
	if s.useStoredProcedures {
		// Best effort: if legacy tables are missing in target DB, the transaction still succeeds for ITMCOM_* tables.
		_, _ = tx.ExecContext(ctx, `DELETE FROM Config`)
		_, _ = tx.ExecContext(ctx, `DELETE FROM Modem`)
		_, _ = tx.ExecContext(ctx, `DELETE FROM Port`)
		_, _ = tx.ExecContext(ctx, `DELETE FROM Routing`)

		for _, l := range data.Listeners {
			typeID := 1
			if l.ModemType == ModemTypeATSWP {
				typeID = 4
			}
			_, _ = tx.ExecContext(ctx, `
INSERT INTO Config (IPAddress, PortNumber, ServerID, mbCurrent, TypeId)
VALUES (@p1, @p2, @p3, 1, @p4)`, l.Address, l.Port, l.ServerID, typeID)
		}
		for _, r := range data.Routing {
			_, _ = tx.ExecContext(ctx, `
INSERT INTO Modem (ModemID, SiteID, mbConnect, ConnectionTime, Flag)
VALUES (@p1, @p2, 0, GETDATE(), 0)`, r.ModemID, r.SiteID)
		}
		for _, p := range data.COMPorts {
			parityInt := 0
			switch p.Parity {
			case "1", "odd", "Odd":
				parityInt = 1
			case "2", "even", "Even":
				parityInt = 2
			}
			_, _ = tx.ExecContext(ctx, `
INSERT INTO Port (PortName, BaudRate, Parity, DataBits, StopBits)
VALUES (@p1, @p2, @p3, @p4, @p5)`,
				p.PortName, p.BaudRate, parityInt, p.DataBits, p.StopBits)
		}
		for _, r := range data.COMRoutes {
			_, _ = tx.ExecContext(ctx, `
INSERT INTO Routing (SiteID, PortID, BeginRange, EndRange, LocalSiteID)
VALUES (@p1, @p2, @p3, @p4, @p5)`,
				r.SiteID, r.PortID, r.BeginRange, r.EndRange, r.LocalSiteID)
		}
	}

	return tx.Commit()
}

func (s *mssqlStore) SaveConnectionEvent(ctx context.Context, modemID string, siteID int, connected bool, eventTime time.Time) error {
	if s.useStoredProcedures {
		msg, status := legacyConnectionLogPayload(modemID, connected)
		_, err := s.db.ExecContext(ctx, `
EXECUTE dbo.InsertLog @SiteID=@p1, @COMPort=@p2, @Message=@p3, @SatusId=@p4, @Time=@p5, @RecordsCount=@p6;`,
			siteID, "", msg, status, eventTime, legacyLogRecordsCnt)
		if err == nil {
			return nil
		}
	}
	_, err := s.db.ExecContext(ctx, `
INSERT INTO ITMCOM_ConnectionEvents (ModemID, SiteID, Connected, EventTime)
VALUES (@p1, @p2, @p3, @p4)`,
		modemID, siteID, connected, eventTime)
	return err
}

func (s *mssqlStore) SaveFrame(ctx context.Context, frame BatchRecord) error {
	if s.useStoredProcedures {
		msg, status := legacyFrameLogPayload(frame)
		_, err := s.db.ExecContext(ctx, `
EXECUTE dbo.InsertLog @SiteID=@p1, @COMPort=@p2, @Message=@p3, @SatusId=@p4, @Time=@p5, @RecordsCount=@p6;`,
			frame.SiteIDFrom, "", msg, status, frame.Timestamp, legacyLogRecordsCnt)
		if err == nil {
			return nil
		}
	}
	_, err := s.db.ExecContext(ctx, `
INSERT INTO ITMCOM_Frames (
	ModemID, ModemType, SiteIDFrom, SiteIDTo, LinkID, BatchHex, EventTime
) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7)`,
		frame.ModemID, frame.ModemType.String(), frame.SiteIDFrom, frame.SiteIDTo, int(frame.LinkID), frame.BatchHex, frame.Timestamp)
	return err
}

func legacyConnectionLogPayload(modemID string, connected bool) (string, int) {
	m := "MODEM_DISCONNECTED " + modemID
	if connected {
		m = "MODEM_CONNECTED " + modemID
	}
	return m, legacyLogStatusInfo
}

func legacyFrameLogPayload(frame BatchRecord) (string, int) {
	msg := fmt.Sprintf(
		"FRAME modem=%s type=%s siteFrom=%d siteTo=%d link=%d hex=%s",
		frame.ModemID,
		frame.ModemType.String(),
		frame.SiteIDFrom,
		frame.SiteIDTo,
		int(frame.LinkID),
		frame.BatchHex,
	)
	return msg, legacyLogStatusInfo
}

func (s *mssqlStore) loadListeners(ctx context.Context, rd *RuntimeData) error {
	rows, err := s.queryRuntimeRows(ctx,
		`SELECT ConfigurationId, Address, Port, ServerID, ModemType FROM ITMCOM_Listeners WHERE IsCurrent = 1`,
		`SELECT ConfigId, IPAddress, PortNumber, ServerID, TypeId FROM Config WHERE mbCurrent = 1`,
		`EXECUTE dbo.SelectConfig`,
	)
	if err != nil {
		return err
	}
	defer rows.Close()
	for rows.Next() {
		m, err := scanRowMap(rows)
		if err != nil {
			return err
		}
		cfg := ListenerConfig{
			ConfigurationId: asInt(m, "configurationid", "configid"),
			Address:         asString(m, "address", "ipaddress"),
			Port:            asInt(m, "port", "portnumber"),
			ServerID:        asString(m, "serverid"),
		}
		if t := asString(m, "modemtype"); t != "" {
			mt, err := parseModemType(t)
			if err != nil {
				mt = ModemTypeLegacy
			}
			cfg.ModemType = mt
		} else {
			switch asInt(m, "typeid") {
			case 4:
				cfg.ModemType = ModemTypeATSWP
			default:
				cfg.ModemType = ModemTypeLegacy
			}
		}
		rd.Listeners = append(rd.Listeners, cfg)
	}
	return nil
}

func (s *mssqlStore) loadRouting(ctx context.Context, rd *RuntimeData) error {
	rows, err := s.queryRuntimeRows(ctx,
		`SELECT SiteID, ModemID FROM ITMCOM_Routing`,
		`SELECT SiteID, ModemID FROM Modem`,
		`EXECUTE dbo.SelectModem`,
	)
	if err != nil {
		return err
	}
	defer rows.Close()
	for rows.Next() {
		m, err := scanRowMap(rows)
		if err != nil {
			return err
		}
		rd.Routing = append(rd.Routing, RoutingConfig{
			SiteID:  asInt(m, "siteid"),
			ModemID: asString(m, "modemid"),
		})
	}
	return nil
}

func (s *mssqlStore) loadCOMPorts(ctx context.Context, rd *RuntimeData) error {
	rows, err := s.queryRuntimeRows(ctx,
		`SELECT PortID, PortName, BaudRate, DataBits, StopBits, Parity FROM ITMCOM_COMPorts`,
		`SELECT PortId, PortName, BaudRate, DataBits, StopBits, Parity FROM Port`,
		`EXECUTE dbo.SelectPort`,
	)
	if err != nil {
		return err
	}
	defer rows.Close()
	for rows.Next() {
		m, err := scanRowMap(rows)
		if err != nil {
			return err
		}
		rd.COMPorts = append(rd.COMPorts, COMPortConfig{
			PortID:   asInt(m, "portid"),
			PortName: asString(m, "portname"),
			BaudRate: asInt(m, "baudrate"),
			DataBits: asInt(m, "databits"),
			StopBits: asInt(m, "stopbits"),
			Parity:   asString(m, "parity"),
		})
		if rd.COMPorts[len(rd.COMPorts)-1].Parity == "" {
			rd.COMPorts[len(rd.COMPorts)-1].Parity = "none"
		}
	}
	return nil
}

func (s *mssqlStore) loadCOMRoutes(ctx context.Context, rd *RuntimeData) error {
	rows, err := s.queryRuntimeRows(ctx,
		`SELECT SiteID, PortID, BeginRange, EndRange, LocalSiteID FROM ITMCOM_COMRoutes`,
		`SELECT SiteID, PortID, BeginRange, EndRange, LocalSiteID FROM Routing`,
		`EXECUTE dbo.SelectRouting`,
	)
	if err != nil {
		return err
	}
	defer rows.Close()
	for rows.Next() {
		m, err := scanRowMap(rows)
		if err != nil {
			return err
		}
		rd.COMRoutes = append(rd.COMRoutes, COMRouteConfig{
			SiteID:      asInt(m, "siteid"),
			PortID:      asInt(m, "portid"),
			BeginRange:  asInt(m, "beginrange"),
			EndRange:    asInt(m, "endrange"),
			LocalSiteID: asInt(m, "localsiteid"),
		})
	}
	return nil
}

func (s *mssqlStore) queryRuntimeRows(ctx context.Context, modernSQL, legacySQL, legacyProc string) (*sql.Rows, error) {
	rows, err := s.db.QueryContext(ctx, modernSQL)
	if err == nil {
		return rows, nil
	}
	if s.useStoredProcedures {
		if rows, procErr := s.db.QueryContext(ctx, legacyProc); procErr == nil {
			return rows, nil
		}
	}
	return s.db.QueryContext(ctx, legacySQL)
}

func scanRowMap(rows *sql.Rows) (map[string]any, error) {
	cols, err := rows.Columns()
	if err != nil {
		return nil, err
	}
	raw := make([]any, len(cols))
	ptrs := make([]any, len(cols))
	for i := range raw {
		ptrs[i] = &raw[i]
	}
	if err := rows.Scan(ptrs...); err != nil {
		return nil, err
	}
	out := make(map[string]any, len(cols))
	for i, c := range cols {
		out[strings.ToLower(c)] = raw[i]
	}
	return out, nil
}

func asString(m map[string]any, keys ...string) string {
	for _, k := range keys {
		v, ok := m[strings.ToLower(k)]
		if !ok || v == nil {
			continue
		}
		switch x := v.(type) {
		case string:
			return x
		case []byte:
			return string(x)
		default:
			return fmt.Sprintf("%v", x)
		}
	}
	return ""
}

func asInt(m map[string]any, keys ...string) int {
	for _, k := range keys {
		v, ok := m[strings.ToLower(k)]
		if !ok || v == nil {
			continue
		}
		switch x := v.(type) {
		case int:
			return x
		case int64:
			return int(x)
		case int32:
			return int(x)
		case float64:
			return int(x)
		case []byte:
			if n, err := strconv.Atoi(string(x)); err == nil {
				return n
			}
		case string:
			if n, err := strconv.Atoi(x); err == nil {
				return n
			}
		}
	}
	return 0
}

func (s *mssqlStore) SaveStatisticsSnapshot(ctx context.Context, states []ModemState, at time.Time) error {
	tx, err := s.db.BeginTx(ctx, nil)
	if err != nil {
		return err
	}
	defer func() {
		_ = tx.Rollback()
	}()
	for _, st := range states {
		if _, err := tx.ExecContext(ctx, `
INSERT INTO ITMCOM_StatisticsSnapshots (
	ModemID, SiteID, Connected, InTraffic, OutTraffic, QueueDepth, SnapshotTime
) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7)`,
			st.ModemID, st.SiteID, st.Connected, st.InTraffic, st.OutTraffic, st.QueueDepth, at); err != nil {
			return err
		}
	}
	return tx.Commit()
}

func (s *mssqlStore) PruneOldData(ctx context.Context, olderThan time.Time) error {
	queries := []string{
		`DELETE FROM ITMCOM_Frames WHERE EventTime < @p1`,
		`DELETE FROM ITMCOM_ConnectionEvents WHERE EventTime < @p1`,
		`DELETE FROM ITMCOM_StatisticsSnapshots WHERE SnapshotTime < @p1`,
	}
	for _, q := range queries {
		_, _ = s.db.ExecContext(ctx, q, olderThan)
	}
	return nil
}

func (s *mssqlStore) migrate(ctx context.Context) error {
	stmts := []string{
		`IF OBJECT_ID('ITMCOM_Listeners', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_Listeners (
		ConfigurationId INT IDENTITY(1,1) PRIMARY KEY,
		Address NVARCHAR(64) NOT NULL,
		Port INT NOT NULL,
		ServerID NVARCHAR(64) NOT NULL,
		ModemType NVARCHAR(16) NOT NULL,
		IsCurrent BIT NOT NULL DEFAULT(1)
	);
END`,
		`IF OBJECT_ID('ITMCOM_Routing', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_Routing (
		SiteID INT NOT NULL,
		ModemID NVARCHAR(64) NOT NULL PRIMARY KEY
	);
END`,
		`IF OBJECT_ID('ITMCOM_COMPorts', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_COMPorts (
		PortID INT IDENTITY(1,1) PRIMARY KEY,
		PortName NVARCHAR(32) NOT NULL,
		BaudRate INT NOT NULL,
		DataBits INT NOT NULL,
		StopBits INT NOT NULL,
		Parity NVARCHAR(16) NOT NULL DEFAULT('none')
	);
END`,
		`IF OBJECT_ID('ITMCOM_COMRoutes', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_COMRoutes (
		SiteID INT NOT NULL,
		PortID INT NOT NULL,
		BeginRange INT NOT NULL,
		EndRange INT NOT NULL,
		LocalSiteID INT NOT NULL
	);
END`,
		`IF OBJECT_ID('ITMCOM_ConnectionEvents', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_ConnectionEvents (
		Id BIGINT IDENTITY(1,1) PRIMARY KEY,
		ModemID NVARCHAR(64) NOT NULL,
		SiteID INT NOT NULL,
		Connected BIT NOT NULL,
		EventTime DATETIME2 NOT NULL
	);
	CREATE INDEX IX_ITMCOM_ConnectionEvents_ModemID_EventTime ON ITMCOM_ConnectionEvents(ModemID, EventTime DESC);
END`,
		`IF OBJECT_ID('ITMCOM_Frames', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_Frames (
		Id BIGINT IDENTITY(1,1) PRIMARY KEY,
		ModemID NVARCHAR(64) NOT NULL,
		ModemType NVARCHAR(16) NOT NULL,
		SiteIDFrom INT NOT NULL,
		SiteIDTo INT NOT NULL,
		LinkID INT NOT NULL,
		BatchHex NVARCHAR(MAX) NOT NULL,
		EventTime DATETIME2 NOT NULL
	);
	CREATE INDEX IX_ITMCOM_Frames_ModemID_EventTime ON ITMCOM_Frames(ModemID, EventTime DESC);
END`,
		`IF OBJECT_ID('ITMCOM_StatisticsSnapshots', 'U') IS NULL
BEGIN
	CREATE TABLE ITMCOM_StatisticsSnapshots (
		Id BIGINT IDENTITY(1,1) PRIMARY KEY,
		ModemID NVARCHAR(64) NOT NULL,
		SiteID INT NOT NULL,
		Connected BIT NOT NULL,
		InTraffic BIGINT NOT NULL,
		OutTraffic BIGINT NOT NULL,
		QueueDepth INT NOT NULL,
		SnapshotTime DATETIME2 NOT NULL
	);
	CREATE INDEX IX_ITMCOM_StatisticsSnapshots_ModemID_SnapshotTime ON ITMCOM_StatisticsSnapshots(ModemID, SnapshotTime DESC);
END`,
	}
	for _, q := range stmts {
		if _, err := s.db.ExecContext(ctx, q); err != nil {
			return fmt.Errorf("migrate failed: %w", err)
		}
	}
	return nil
}

