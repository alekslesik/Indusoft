APP_NAME := indusoft-itmcomd
BIN := itmcomd
MAIN_PKG := ./cmd/itmcomd
GO_FILES := ./...
DOCKER_IMAGE := $(APP_NAME)
ENV_FILE := .env
VERSION ?=
GOTOOLCHAIN_LOCAL := go1.26.2

.DEFAULT_GOAL := help

.PHONY: help all run alesklesik1990 build deps fmt fmt-check imports lint vet staticcheck golangci-lint test docker-build docker-run docker-stop docker-logs docker-compose-up docker-compose-down preprod vuln tag-create tag-push tag-release

help:
	@echo "Available make targets:"
	@echo "  help          - Show this help"
	@echo "  run           - Run service locally (uses .env if present)"
	@echo "  alesklesik1990- Alias for run"
	@echo "  build         - Build Go binary"
	@echo "  deps          - Tidy Go modules (go mod tidy)"
	@echo "  fmt           - Format code with gofmt"
	@echo "  fmt-check     - Check formatting with gofmt -l"
	@echo "  imports       - Organize imports with goimports"
	@echo "  lint          - Run all linters (fmt, vet, staticcheck, golangci-lint)"
	@echo "  vet           - Run go vet"
	@echo "  staticcheck   - Run staticcheck (go run, same Go as module)"
	@echo "  golangci-lint - Run golangci-lint (if installed)"
	@echo "  test          - Run go tests"
	@echo "  vuln          - Run govulncheck (via go run)"
	@echo "  docker-build  - Build Docker image (if Dockerfile exists)"
	@echo "  docker-run    - Run service in Docker with .env"
	@echo "  docker-stop   - Stop running Docker container"
	@echo "  docker-logs   - Show Docker logs"
	@echo "  docker-compose-up   - Run service via docker-compose (uses .env)"
	@echo "  docker-compose-down - Stop service started by docker-compose"
	@echo "  preprod       - Full pre-production checks"
	@echo "  tag-create    - Create annotated git tag (v=vX.Y.Z)"
	@echo "  tag-push      - Push git tag to origin (v=vX.Y.Z)"
	@echo "  tag-release   - Create and push tag (v=vX.Y.Z)"

all: preprod

deps:
	go mod tidy

fmt:
	gofmt -w .

fmt-check:
	@unformatted="$$(gofmt -l .)"; \
	if [ -n "$$unformatted" ]; then \
		echo "These files are not gofmt-formatted:"; \
		echo "$$unformatted"; \
		echo ""; \
		echo "Run: make fmt"; \
		exit 1; \
	fi

imports:
	@if command -v goimports >/dev/null 2>&1; then \
		goimports -w .; \
	else \
		GOTOOLCHAIN=$(GOTOOLCHAIN_LOCAL) go run golang.org/x/tools/cmd/goimports@latest -w .; \
	fi

build: deps
	go build -o $(BIN) $(MAIN_PKG)

run:
	@if [ -f $(ENV_FILE) ]; then \
		echo "Loading env from $(ENV_FILE)"; \
		set -a; . ./$(ENV_FILE); set +a; \
	fi; \
	go run $(MAIN_PKG)

alesklesik1990: run

lint: fmt vet staticcheck golangci-lint

vet:
	go vet $(GO_FILES)

staticcheck:
	@echo "staticcheck $(GO_FILES)"
	@GOTOOLCHAIN=$(GOTOOLCHAIN_LOCAL) go run honnef.co/go/tools/cmd/staticcheck@latest $(GO_FILES)

golangci-lint:
	@echo "golangci-lint $(GO_FILES)";
	@if command -v golangci-lint >/dev/null 2>&1; then \
		if ! golangci-lint run ./...; then \
			echo "golangci-lint failed (likely built with older Go). To update, run:"; \
			echo "  go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest"; \
			exit 1; \
		fi; \
	else \
		echo "golangci-lint not found, install with: go install github.com/golangci/golangci-lint/cmd/golangci-lint@latest"; \
	fi

test:
	go test $(GO_FILES)

vuln:
	@echo "govulncheck ./..."
	@GOTOOLCHAIN=$(GOTOOLCHAIN_LOCAL) go run golang.org/x/vuln/cmd/govulncheck@latest ./...

docker-build:
	@if [ -f Dockerfile ]; then \
		docker build -t $(DOCKER_IMAGE) .; \
	else \
		echo "Dockerfile not found, skipping docker build"; \
	fi

docker-run: docker-build
	@if [ -f Dockerfile ]; then \
		docker run --rm --env-file $(ENV_FILE) --name $(APP_NAME) $(DOCKER_IMAGE); \
	else \
		echo "Dockerfile not found, skipping docker run"; \
	fi

docker-stop:
	- docker stop $(APP_NAME) 2>/dev/null || true

docker-logs:
	docker logs -f $(APP_NAME)

docker-compose-up:
	docker compose up --build

docker-compose-down:
	docker compose down

preprod: deps fmt imports vet staticcheck golangci-lint test vuln docker-build
	@echo "Pre-production checks completed successfully."

tag-create:
	@if [ -z "$(v)" ]; then \
		echo "Usage: make tag-create v=vX.Y.Z"; \
		exit 1; \
	fi
	git tag -a "$(v)" -m "Release $(v)"

tag-push:
	@if [ -z "$(v)" ]; then \
		echo "Usage: make tag-push v=vX.Y.Z"; \
		exit 1; \
	fi
	git push origin "$(v)"

tag-release: tag-create tag-push
