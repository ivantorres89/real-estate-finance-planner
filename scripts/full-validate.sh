#!/usr/bin/env bash
# full-validate.sh - Full E2E validation for Real Estate Finance Planner
# Usage: ./scripts/full-validate.sh [--skip-build] [--skip-unit-tests] [--headed]
# Run from the repository root directory.

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SKIP_BUILD=false
SKIP_UNIT=false
HEADED=""

for arg in "$@"; do
  case "$arg" in
    --skip-build) SKIP_BUILD=true ;;
    --skip-unit-tests) SKIP_UNIT=true ;;
    --headed) HEADED="--headed" ;;
  esac
done

echo "=== Real Estate Finance Planner - Full Validation ==="
echo ""

# ------------------------------------------------------------------
# 1. Backend build
# ------------------------------------------------------------------
if [ "$SKIP_BUILD" = false ]; then
  echo "[1/6] Building backend..."
  cd "$ROOT/src/backend"
  dotnet build RealEstateFinancePlanner.slnx --nologo -v q
  echo "  Backend build OK"
fi

# ------------------------------------------------------------------
# 2. Unit tests
# ------------------------------------------------------------------
if [ "$SKIP_UNIT" = false ]; then
  echo "[2/6] Running unit tests..."
  cd "$ROOT/src/backend"
  dotnet test tests/RealEstateFinancePlanner.Tests.Unit --nologo -v q --no-build
  echo "  Unit tests OK"
fi

# ------------------------------------------------------------------
# 3. Frontend build
# ------------------------------------------------------------------
if [ "$SKIP_BUILD" = false ]; then
  echo "[3/6] Building frontend..."
  cd "$ROOT/src/frontend"
  npx ng build --configuration=production > /dev/null 2>&1
  echo "  Frontend build OK"
fi

# ------------------------------------------------------------------
# 4. Docker Compose up
# ------------------------------------------------------------------
echo "[4/6] Starting Docker Compose stack..."
cd "$ROOT"
docker compose down --remove-orphans > /dev/null 2>&1 || true
docker compose up --build -d

# ------------------------------------------------------------------
# 5. Wait for services
# ------------------------------------------------------------------
echo "[5/6] Waiting for services to be healthy..."
MAX_WAIT=120
ELAPSED=0

while [ "$ELAPSED" -lt "$MAX_WAIT" ]; do
  if curl -sf http://localhost:5000/health > /dev/null 2>&1 && \
     curl -sf http://localhost:4200 > /dev/null 2>&1; then
    echo "  All services healthy"
    break
  fi
  sleep 3
  ELAPSED=$((ELAPSED + 3))
  echo "  ...waiting (${ELAPSED}s)"
done

if [ "$ELAPSED" -ge "$MAX_WAIT" ]; then
  echo "  ERROR: Services did not become healthy within ${MAX_WAIT}s"
  docker compose logs --tail=50
  exit 1
fi

# ------------------------------------------------------------------
# 6. Playwright E2E tests
# ------------------------------------------------------------------
echo "[6/6] Running Playwright E2E tests..."
cd "$ROOT/tests/e2e"
export BASE_URL="http://localhost:4200"
export BACKEND_URL="http://localhost:5000"

npx playwright test $HEADED
E2E_RESULT=$?

# ------------------------------------------------------------------
# Summary
# ------------------------------------------------------------------
echo ""
echo "=== Validation Summary ==="
if [ "$E2E_RESULT" -eq 0 ]; then
  echo "  ALL CHECKS PASSED"
else
  echo "  E2E TESTS FAILED (exit code: $E2E_RESULT)"
  echo "  Run 'npx playwright show-report' in tests/e2e for details"
fi

echo ""
echo "To stop the stack: docker compose down"
echo ""

exit $E2E_RESULT
