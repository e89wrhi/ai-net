<#
.SYNOPSIS
    AI-Net Main Entry Point — orchestrates clean, build, migrations, and execution.

.DESCRIPTION
    Windows PowerShell equivalent of scripts/entrypoint.sh.
    Pass -SkipClean to skip the dotnet clean step on subsequent runs.
    Pass -RunMigrations to apply EF Core migrations before starting.

.EXAMPLE
    # Default: clean → restore → build → run
    .\scripts\entrypoint.ps1

    # Skip clean, apply migrations, set environment
    .\scripts\entrypoint.ps1 -SkipClean -RunMigrations -Environment Staging

.PARAMETER SkipClean
    Skip the 'dotnet clean' step. Useful for faster subsequent runs.

.PARAMETER RunMigrations
    Run Update-Databases.ps1 before starting the application.

.PARAMETER Environment
    Sets ASPNETCORE_ENVIRONMENT. Defaults to 'Development'.
#>

param(
    [switch]$SkipClean,
    [switch]$RunMigrations,
    [string]$Environment = "Development"
)

$ErrorActionPreference = "Stop"

$scriptPath  = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath    = Split-Path -Parent $scriptPath
$solutionFile = Join-Path $rootPath "AI.sln"
$apiProject   = Join-Path $rootPath "Src\Api\AI.Api\AI.Api.csproj"
$updateDbScript = Join-Path $scriptPath "Update-Databases.ps1"

function Log   { param($msg) Write-Host $msg -ForegroundColor Cyan }
function Step  { param($n, $msg) Write-Host "`n-- Step ${n}: $msg" -ForegroundColor Yellow }
function Ok    { param($msg) Write-Host "[OK] $msg" -ForegroundColor Green }
function Err   { param($msg) Write-Host "[ERR] $msg" -ForegroundColor Red }

Log "---------- AI-Net Build System -------------"
Log "Environment : $Environment"
Log "Root        : $rootPath"
Log "Solution    : $solutionFile"

# -- Step 1: Clean ------------------------------------------------------------
if (-not $SkipClean) {
    Step 1 "Cleaning solution..."
    dotnet clean $solutionFile -c Release
    if ($LASTEXITCODE -ne 0) { Err "Clean failed."; exit 1 }
    Ok "Clean complete."
} else {
    Step 1 "Skipping clean [-SkipClean]"
}

# -- Step 2: Restore .NET tools ───────────────────────────────────────────────
Step 2 "Restoring .NET local tools..."
dotnet tool restore
if ($LASTEXITCODE -ne 0) { Err "Tool restore failed."; exit 1 }
Ok "Tools restored."

# -- Step 3: Restore & Build ──────────────────────────────────────────────────
Step 3 "Restoring NuGet packages..."
dotnet restore $solutionFile
if ($LASTEXITCODE -ne 0) { Err "Restore failed."; exit 1 }

Step 3 "Building solution (Release)..."
dotnet build $solutionFile -c Release --no-restore
if ($LASTEXITCODE -ne 0) { Err "Build failed."; exit 1 }
Ok "Build succeeded."

# -- Step 4: Migrations ───────────────────────────────────────────────────────
if ($RunMigrations) {
    Step 4 "Applying EF Core migrations for all modules..."
    & $updateDbScript
    if ($LASTEXITCODE -ne 0) { Err "One or more migrations failed."; exit 1 }
    Ok "Migrations applied."
} else {
    Step 4 "Skipping migrations [pass -RunMigrations to apply]"
}

# -- Step 5: Run ──────────────────────────────────────────────────────────────
Step 5 "Starting AI.Api ($Environment)..."
$env:ASPNETCORE_ENVIRONMENT = $Environment
dotnet run --project $apiProject --no-build --no-restore
