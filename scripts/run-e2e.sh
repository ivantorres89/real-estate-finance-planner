#!/usr/bin/env bash
# run-e2e.sh - Run Playwright E2E tests only (assumes stack is running)
# Usage: ./scripts/run-e2e.sh [--headed]

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
HEADED=""

for arg in "$@"; do
  case "$arg" in
    --headed) HEADED="--headed" ;;
  esac
done

echo "=== Running Playwright E2E Tests ==="

# Quick health check
if ! curl -sf http://localhost:5000/health > /dev/null 2>&1; then
  echo "ERROR: Backend health check failed at http://localhost:5000/health"
  echo "Make sure the stack is running: docker compose up -d"
  exit 1
fi

if ! curl -sf http://localhost:4200 > /dev/null 2>&1; then
  echo "ERROR: Frontend not responding at http://localhost:4200"
  echo "Make sure the stack is running: docker compose up -d"
  exit 1
fi

echo "Services are up. Running tests..."

cd "$ROOT/tests/e2e"
export BASE_URL="http://localhost:4200"
export BACKEND_URL="http://localhost:5000"

npx playwright test $HEADED
