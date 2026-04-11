# restart-stack.ps1
# Stops and removes backend + frontend containers (with their images and build cache),
# then brings the full stack back up.
# The mongo-data volume is preserved so database state is NOT lost.

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "--- Stopping all services..." -ForegroundColor Cyan
docker compose down

Write-Host "--- Removing backend and frontend images..." -ForegroundColor Cyan
$images = @('refp-backend', 'refp-frontend')
foreach ($img in $images) {
    $exists = docker images -q $img 2>$null
    if ($exists) {
        docker rmi --force $img
        Write-Host "  Removed image: $img" -ForegroundColor Gray
    }
}

Write-Host "--- Pruning build cache..." -ForegroundColor Cyan
docker builder prune --force

Write-Host "--- Starting stack (no-cache rebuild)..." -ForegroundColor Cyan
docker compose up --build --no-deps -d

Write-Host "--- Done. Stack is up." -ForegroundColor Green
Write-Host "  Frontend : http://localhost:4200"
Write-Host "  Backend  : http://localhost:5000"
Write-Host "  MongoDB  : localhost:27017  (volume mongo-data preserved)"
