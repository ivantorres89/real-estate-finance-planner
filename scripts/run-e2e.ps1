# run-e2e.ps1 - Run only Playwright E2E tests (assumes stack is already running)
# Usage: .\scripts\run-e2e.ps1 [-Headed]

param(
    [switch]$Headed
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

$env:BASE_URL = "http://localhost:4200"
$env:BACKEND_URL = "http://localhost:5000"

Push-Location "$root\tests\e2e"

if ($Headed) {
    npx playwright test --headed
} else {
    npx playwright test
}

$result = $LASTEXITCODE
Pop-Location

if ($result -ne 0) {
    Write-Host "E2E tests failed. Run 'npx playwright show-report' in tests/e2e for details." -ForegroundColor Red
}

exit $result
