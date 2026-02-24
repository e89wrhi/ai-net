$root = Get-Location
$apiPath = Join-Path $root "Src/Api/AI.Api/AI.Api.csproj"
$modulesRoot = Join-Path $root "Src/Modules"

Write-Host "🚀 Starting Auto-Discovery for Database Updates..." -ForegroundColor Cyan

$projects = Get-ChildItem -Path $modulesRoot -Filter *.csproj -Recurse

foreach ($project in $projects) {
    $dataDir = Join-Path $project.Directory.FullName "Data"
    if (Test-Path $dataDir) {
        $contextFiles = Get-ChildItem -Path $dataDir -Filter "*Context.cs"
        
        foreach ($contextFile in $contextFiles) {
            $contextName = $contextFile.BaseName
            Write-Host "----------------------------------------------------"
            Write-Host "📦 Module: $($project.Directory.Name)" -ForegroundColor Yellow
            Write-Host "🆙 Updating: $contextName"
            
            try {
                dotnet ef database update `
                    --project $project.FullName `
                    --startup-project $apiPath `
                    --context $contextName
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Database updated successfully for $contextName" -ForegroundColor Green
                } else {
                    Write-Host "❌ Error occurred during database update for $contextName" -ForegroundColor Red
                }
            } catch {
                Write-Host "❌ Failed to update database for $contextName: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
}

Write-Host "----------------------------------------------------"
Write-Host "✨ All databases processed!" -ForegroundColor Cyan
