package itmcom

import (
	"encoding/json"
	"sync"
	"time"
)

type MonitorQueue struct {
	mu    sync.Mutex
	max   int
	inUse bool
	head  int
	size  int
	buf   []BatchRecord
}

func NewMonitorQueue(max int) *MonitorQueue {
	if max <= 0 {
		max = 10000
	}
	return &MonitorQueue{
		max:   max,
		inUse: true,
		buf:   make([]BatchRecord, max),
	}
}

func (q *MonitorQueue) SetInUse(inUse bool) {
	q.mu.Lock()
	defer q.mu.Unlock()
	q.inUse = inUse
	if !inUse {
		q.head = 0
		q.size = 0
	}
}

// Push enqueues a record and keeps at most max records (oldest are dropped).
func (q *MonitorQueue) Push(r BatchRecord) {
	q.mu.Lock()
	defer q.mu.Unlock()
	if !q.inUse {
		return
	}
	if q.max == 0 {
		return
	}
	if q.size < q.max {
		idx := (q.head + q.size) % q.max
		q.buf[idx] = r
		q.size++
		return
	}
	// Overwrite oldest at head.
	q.buf[q.head] = r
	q.head = (q.head + 1) % q.max
}

func (q *MonitorQueue) SnapshotAndClear() []BatchRecord {
	q.mu.Lock()
	defer q.mu.Unlock()
	if q.size == 0 {
		return nil
	}
	out := make([]BatchRecord, 0, q.size)
	for i := 0; i < q.size; i++ {
		idx := (q.head + i) % q.max
		out = append(out, q.buf[idx])
	}
	// Clear queue.
	q.head = 0
	q.size = 0
	return out
}

func (q *MonitorQueue) Snapshot(clear bool) []BatchRecord {
	q.mu.Lock()
	defer q.mu.Unlock()
	if q.size == 0 {
		return nil
	}
	out := make([]BatchRecord, 0, q.size)
	for i := 0; i < q.size; i++ {
		idx := (q.head + i) % q.max
		out = append(out, q.buf[idx])
	}
	if clear {
		q.head = 0
		q.size = 0
	}
	return out
}

func (q *MonitorQueue) Len() int {
	q.mu.Lock()
	defer q.mu.Unlock()
	return q.size
}

func (q *MonitorQueue) InUse() bool {
	q.mu.Lock()
	defer q.mu.Unlock()
	return q.inUse
}

func (r BatchRecord) MarshalJSON() ([]byte, error) {
	// Keep encoding simple/explicit (avoid serializing Raw byte slice by default).
	// This method is optional; leaving it out would include Raw.
	type view struct {
		Timestamp  time.Time `json:"timestamp"`
		ModemID    string    `json:"modemID"`
		ModemType  string    `json:"modemType"`
		SiteIDFrom int       `json:"siteIDFrom"`
		SiteIDTo   int       `json:"siteIDTo"`
		LinkID     byte      `json:"linkID"`
		BatchHex   string    `json:"batchHex"`
	}
	v := view{
		Timestamp:  r.Timestamp,
		ModemID:    r.ModemID,
		ModemType:  r.ModemType.String(),
		SiteIDFrom: r.SiteIDFrom,
		SiteIDTo:   r.SiteIDTo,
		LinkID:     r.LinkID,
		BatchHex:   r.BatchHex,
	}
	// encoding/json will handle it.
	return json.Marshal(v)
}
