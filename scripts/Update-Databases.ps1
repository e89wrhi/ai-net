param(
    [string]$PgHost = "localhost",
    [int]$PgPort = 5431,
    [string]$PgUser = "postgres",
    [string]$PgPassword = "changeme"
)

$root = Get-Location
$apiPath = Join-Path $root "Src/Api/AI.Api/AI.Api.csproj"
$modulesRoot = Join-Path $root "Src/Modules"

$env:PGPASSWORD = $PgPassword

Write-Host ">>> Starting Database Migration Runner..." -ForegroundColor Cyan
Write-Host "    Target: ${PgHost}:${PgPort} (user=$PgUser)" -ForegroundColor Gray

# Pre-flight: verify Postgres is reachable
Write-Host ""
Write-Host "Checking Postgres connectivity at ${PgHost}:${PgPort}..." -ForegroundColor Gray
try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $connect = $tcpClient.BeginConnect($PgHost, $PgPort, $null, $null)
    $wait = $connect.AsyncWaitHandle.WaitOne(3000, $false)
    if (-not $wait) {
        $tcpClient.Close()
        Write-Host ""
        Write-Host "[FATAL] Cannot reach Postgres at ${PgHost}:${PgPort} - is Docker running?" -ForegroundColor Red
        Write-Host "        Start services first: docker compose up -d postgres" -ForegroundColor Yellow
        exit 1
    }
    $tcpClient.EndConnect($connect)
    $tcpClient.Close()
    Write-Host "[OK] Postgres is reachable." -ForegroundColor Green
} catch {
    Write-Host ""
    Write-Host "[FATAL] Postgres connectivity check failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

$successCount = 0
$failureCount = 0

$projects = Get-ChildItem -Path $modulesRoot -Filter *.csproj -Recurse

foreach ($project in $projects) {
    $dataDir = Join-Path $project.Directory.FullName "Data"
    if (Test-Path $dataDir) {
        $contextFiles = Get-ChildItem -Path $dataDir -Filter "*Context.cs"
        foreach ($contextFile in $contextFiles) {
            $contextName = $contextFile.BaseName
            Write-Host "----------------------------------------------------"
            Write-Host "[Module]: $($project.Directory.Name)" -ForegroundColor Yellow
            Write-Host "-> Updating: $contextName"
            try {
                dotnet ef database update --project $project.FullName --startup-project $apiPath --context $contextName
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "[OK] Database updated successfully for $contextName" -ForegroundColor Green
                    $successCount++
                } else {
                    Write-Host "[ERROR] Error occurred during database update for $contextName" -ForegroundColor Red
                    $failureCount++
                }
            } catch {
                Write-Host "[ERROR] Failed to update database for ${contextName}: $($_.Exception.Message)" -ForegroundColor Red
                $failureCount++
            }
        }
    }
}

Write-Host "----------------------------------------------------"
Write-Host ">>> Done - Success=$successCount, Failed=$failureCount" -ForegroundColor Cyan

if ($failureCount -gt 0) { exit 1 }
