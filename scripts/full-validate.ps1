# full-validate.ps1 - Full E2E validation for Real Estate Finance Planner
# Usage: .\scripts\full-validate.ps1
# Run from the repository root directory.

param(
    [switch]$SkipBuild,
    [switch]$SkipUnitTests,
    [switch]$Headed
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Write-Host "=== Real Estate Finance Planner - Full Validation ===" -ForegroundColor Cyan
Write-Host ""

# ------------------------------------------------------------------
# 1. Backend build
# ------------------------------------------------------------------
if (-not $SkipBuild) {
    Write-Host "[1/6] Building backend..." -ForegroundColor Yellow
    Push-Location "$root\src\backend"
    dotnet build RealEstateFinancePlanner.slnx --nologo -v q
    if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Backend build failed" }
    Pop-Location
    Write-Host "  Backend build OK" -ForegroundColor Green
}

# ------------------------------------------------------------------
# 2. Unit tests
# ------------------------------------------------------------------
if (-not $SkipUnitTests) {
    Write-Host "[2/6] Running unit tests..." -ForegroundColor Yellow
    Push-Location "$root\src\backend"
    dotnet test tests/RealEstateFinancePlanner.Tests.Unit --nologo -v q --no-build
    if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Unit tests failed" }
    Pop-Location
    Write-Host "  Unit tests OK" -ForegroundColor Green
}

# ------------------------------------------------------------------
# 3. Frontend build
# ------------------------------------------------------------------
if (-not $SkipBuild) {
    Write-Host "[3/6] Building frontend..." -ForegroundColor Yellow
    Push-Location "$root\src\frontend"
    npx ng build --configuration=production 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Frontend build failed" }
    Pop-Location
    Write-Host "  Frontend build OK" -ForegroundColor Green
}

# ------------------------------------------------------------------
# 4. Docker Compose up
# ------------------------------------------------------------------
Write-Host "[4/6] Starting Docker Compose stack..." -ForegroundColor Yellow
Push-Location $root
docker compose down --remove-orphans 2>&1 | Out-Null
docker compose up --build -d 2>&1 | Out-String | Write-Host
if ($LASTEXITCODE -ne 0) { Pop-Location; throw "Docker Compose up failed" }
Pop-Location

# ------------------------------------------------------------------
# 5. Wait for services
# ------------------------------------------------------------------
Write-Host "[5/6] Waiting for services to be healthy..." -ForegroundColor Yellow
$maxWait = 120
$elapsed = 0
$ready = $false

while ($elapsed -lt $maxWait) {
    try {
        $resp = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 3 -ErrorAction SilentlyContinue
        if ($resp.StatusCode -eq 200) {
            $resp2 = Invoke-WebRequest -Uri "http://localhost:4200" -UseBasicParsing -TimeoutSec 3 -ErrorAction SilentlyContinue
            if ($resp2.StatusCode -eq 200) {
                $ready = $true
                break
            }
        }
    } catch {}
    Start-Sleep -Seconds 3
    $elapsed += 3
    Write-Host "  ...waiting ($elapsed s)" -ForegroundColor DarkGray
}

if (-not $ready) {
    Write-Host "  Services did not become healthy within $maxWait seconds" -ForegroundColor Red
    Write-Host "  Docker logs:" -ForegroundColor Red
    Push-Location $root
    docker compose logs --tail=50
    Pop-Location
    throw "Stack not ready"
}
Write-Host "  All services healthy" -ForegroundColor Green

# ------------------------------------------------------------------
# 6. Playwright E2E tests
# ------------------------------------------------------------------
Write-Host "[6/6] Running Playwright E2E tests..." -ForegroundColor Yellow
Push-Location "$root\tests\e2e"

$env:BASE_URL = "http://localhost:4200"
$env:BACKEND_URL = "http://localhost:5000"

if ($Headed) {
    npx playwright test --headed
} else {
    npx playwright test
}
$e2eResult = $LASTEXITCODE
Pop-Location

# ------------------------------------------------------------------
# Summary
# ------------------------------------------------------------------
Write-Host ""
Write-Host "=== Validation Summary ===" -ForegroundColor Cyan
if ($e2eResult -eq 0) {
    Write-Host "  ALL CHECKS PASSED" -ForegroundColor Green
} else {
    Write-Host "  E2E TESTS FAILED (exit code: $e2eResult)" -ForegroundColor Red
    Write-Host "  Run 'npx playwright show-report' in tests/e2e for details" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "To stop the stack: docker compose down" -ForegroundColor DarkGray
Write-Host ""

exit $e2eResult
