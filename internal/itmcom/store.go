package itmcom

import (
	"context"
	"time"
)

type RuntimeData struct {
	Listeners []ListenerConfig
	Routing   []RoutingConfig
	COMPorts  []COMPortConfig
	COMRoutes []COMRouteConfig
}

type Store interface {
	Close() error
	LoadRuntimeData(ctx context.Context) (*RuntimeData, error)
	SaveRuntimeData(ctx context.Context, data *RuntimeData) error
	SaveConnectionEvent(ctx context.Context, modemID string, siteID int, connected bool, eventTime time.Time) error
	SaveFrame(ctx context.Context, frame BatchRecord) error
	SaveStatisticsSnapshot(ctx context.Context, states []ModemState, at time.Time) error
	PruneOldData(ctx context.Context, olderThan time.Time) error
}

type noOpStore struct{}

func (s *noOpStore) Close() error { return nil }

func (s *noOpStore) LoadRuntimeData(ctx context.Context) (*RuntimeData, error) {
	_ = ctx
	return nil, nil
}

func (s *noOpStore) SaveRuntimeData(ctx context.Context, data *RuntimeData) error {
	_ = ctx
	_ = data
	return nil
}

func (s *noOpStore) SaveConnectionEvent(ctx context.Context, modemID string, siteID int, connected bool, eventTime time.Time) error {
	_ = ctx
	_ = modemID
	_ = siteID
	_ = connected
	_ = eventTime
	return nil
}

func (s *noOpStore) SaveFrame(ctx context.Context, frame BatchRecord) error {
	_ = ctx
	_ = frame
	return nil
}

func (s *noOpStore) SaveStatisticsSnapshot(ctx context.Context, states []ModemState, at time.Time) error {
	_ = ctx
	_ = states
	_ = at
	return nil
}

func (s *noOpStore) PruneOldData(ctx context.Context, olderThan time.Time) error {
	_ = ctx
	_ = olderThan
	return nil
}
