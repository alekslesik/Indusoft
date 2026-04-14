package itmcom

import (
	"context"
	"testing"

	"github.com/DATA-DOG/go-sqlmock"
)

func TestQueryRuntimeRowsStoredProcedureFirst(t *testing.T) {
	const procSelectConfig = "EXECUTE dbo.SelectConfig"

	db, mock, err := sqlmock.New()
	if err != nil {
		t.Fatalf("sqlmock init: %v", err)
	}
	defer db.Close()

	s := &mssqlStore{db: db, useStoredProcedures: true}
	mock.ExpectQuery(procSelectConfig).WillReturnRows(sqlmock.NewRows([]string{"ConfigId"}).AddRow(1))

	rows, err := s.queryRuntimeRows(context.Background(),
		"SELECT * FROM ITMCOM_Listeners",
		"SELECT * FROM Config",
		procSelectConfig,
	)
	if err != nil {
		t.Fatalf("queryRuntimeRows failed: %v", err)
	}
	_ = rows.Close()
	if err := mock.ExpectationsWereMet(); err != nil {
		t.Fatalf("stored procedure order expectation failed: %v", err)
	}
}

func TestQueryRuntimeRowsModernThenLegacyFallback(t *testing.T) {
	const procSelectConfig = "EXECUTE dbo.SelectConfig"

	db, mock, err := sqlmock.New()
	if err != nil {
		t.Fatalf("sqlmock init: %v", err)
	}
	defer db.Close()

	s := &mssqlStore{db: db, useStoredProcedures: false}
	mock.ExpectQuery("SELECT \\* FROM ITMCOM_Listeners").WillReturnError(context.DeadlineExceeded)
	mock.ExpectQuery("SELECT \\* FROM Config").WillReturnRows(sqlmock.NewRows([]string{"ConfigId"}).AddRow(1))

	rows, err := s.queryRuntimeRows(context.Background(),
		"SELECT * FROM ITMCOM_Listeners",
		"SELECT * FROM Config",
		procSelectConfig,
	)
	if err != nil {
		t.Fatalf("queryRuntimeRows failed: %v", err)
	}
	_ = rows.Close()
	if err := mock.ExpectationsWereMet(); err != nil {
		t.Fatalf("fallback order expectation failed: %v", err)
	}
}
