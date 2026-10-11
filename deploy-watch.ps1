# Wait for Bannerlord/BLSE to exit, then Rebuild+deploy. Intended to run in a Cursor
# background terminal (Shell block_until_ms: 0) — do not Start-Process this into a new window.
$ErrorActionPreference = 'Stop'
Write-Host 'PerkFixesAndRebalance deploy watcher: waiting for Bannerlord/TaleWorlds/BLSE to exit...'
while (Get-Process -ErrorAction SilentlyContinue | Where-Object { $_.ProcessName -match 'Bannerlord|TaleWorlds|BLSE' }) {
    Start-Sleep -Seconds 5
}
Write-Host 'Game closed. Deploying...'
Set-Location $PSScriptRoot
dotnet build -c Release
if ($LASTEXITCODE -eq 0) {
    Write-Host 'DEPLOY_OK'
} else {
    Write-Host "DEPLOY_FAIL exit=$LASTEXITCODE"
    exit $LASTEXITCODE
}
