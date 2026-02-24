param(
    [Parameter(Mandatory=$false)]
    [string]$MigrationName
)

if (-not $MigrationName) {
    $MigrationName = Read-Host "Enter migration name (e.g. Initial)"
}

$root = Get-Location
$apiPath = Join-Path $root "Src/Api/AI.Api/AI.Api.csproj"
$modulesRoot = Join-Path $root "Src/Modules"

Write-Host "🚀 Starting Auto-Discovery for Migrations..." -ForegroundColor Cyan

$projects = Get-ChildItem -Path $modulesRoot -Filter *.csproj -Recurse

foreach ($project in $projects) {
    $dataDir = Join-Path $project.Directory.FullName "Data"
    if (Test-Path $dataDir) {
        # Find files that likely contain a DbContext
        $contextFiles = Get-ChildItem -Path $dataDir -Filter "*Context.cs"
        
        foreach ($contextFile in $contextFiles) {
            $contextName = $contextFile.BaseName
            Write-Host "----------------------------------------------------"
            Write-Host "📦 Module: $($project.Directory.Name)" -ForegroundColor Yellow
            Write-Host "🔍 Context: $contextName"
            
            try {
                dotnet ef migrations add $MigrationName `
                    --project $project.FullName `
                    --startup-project $apiPath `
                    --context $contextName `
                    --output-dir Data/Migrations
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Migration '$MigrationName' added successfully for $contextName" -ForegroundColor Green
                } else {
                    Write-Host "⚠️ No changes detected or error occurred for $contextName" -ForegroundColor DarkYellow
                }
            } catch {
                Write-Host "❌ Failed to add migration for $contextName: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
}

Write-Host "----------------------------------------------------"
Write-Host "✨ Done!" -ForegroundColor Cyan
