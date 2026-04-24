# 🏗️ Automated EF Core Migrations (DbUp / Bundles)

## Overview
Currently, the AI-Net project relies on `Update-Databases.ps1` or `update-db.sh`, which iterate over 20+ DbContexts to apply migrations.

## The Core Problem
1. **Lack of Idempotency**: If the script crashes on database 15 out of 20, the system is left in an unknown, half-migrated state.
2. **Missing CI/CD Integration**: Relying on local developer scripts makes continuous deployment impossible and deeply unsafe for Production.
3. **No Migration Fallback**: If an API update deploys but the migrations fail, the API will crash due to schema mismatches. 

## The Solution: EF Core Bundles in Pre-Deployment Pipelines

### Implementation Steps

#### 1. Generate Executable Bundles
Instead of calling `dotnet ef database update` via a shell script at runtime, use the EF CLI to compile migrations into native executable files:

```bash
# Example for a specific module
dotnet ef migrations bundle \
  --project Src/Modules/CodeGen/CodeGen.csproj \
  --startup-project Src/Api/AI.Api.csproj \
  --context CodeDbContext \
  --output ./publish/migrators/codegen-migrator
```
*Note: In a modular monolith, you would script this in your build pipeline to generate a tight executable for every DbContext.*

#### 2. Alternative: Central DbUp Migration Project
If bundles per-module are tedious, create a generic `AI.DbMigrator` Worker Project. This project references `DbUp` and applies purely SQL scripts iteratively. 

```csharp
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
```

#### 3. Integrate into CI/CD (GitHub Actions / Azure DevOps)
Your deployment pipeline must enforce strict ordering: **Migrate first, Swap traffic second**.

```yaml
# example git-action snippet
jobs:
  migrate_databases:
    runs-on: ubuntu-latest
    steps:
      - name: Run CodeGen Migrator
        run: ./migrators/codegen-migrator --connection "${{ secrets.DB_CONN }}"
      - name: Run ChatBot Migrator
        run: ./migrators/chatbot-migrator --connection "${{ secrets.DB_CONN }}"

  deploy_api:
    needs: migrate_databases  # Will NOT deploy unless all databases succeed
    runs-on: ubuntu-latest
    steps:
      - name: Deploy API container
        run: az containerapp update ...
```

## Outcome
You move from a risky, manual deployment process to an automated, auditable, and inherently safe CI/CD deployment model. If a migration fails, the API image deployment simply aborts—maintaining 100% uptime on the existing API.
