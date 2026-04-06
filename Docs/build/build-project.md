# 🏗️ Local Build & Deployment Guide

Complete step-by-step instructions for running the **AI-Net** platform locally. Follow every section in order for a clean first-time setup.

---

## 📋 Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Clone & Verify the Repository](#2-clone--verify-the-repository)
3. [Install .NET Tooling](#3-install-net-tooling)
4. [Verify Docker](#4-verify-docker)
5. [Restore & Build the Solution](#5-restore--build-the-solution)
6. [Database Migrations](#6-database-migrations)
7. [Run the Full Stack with Aspire](#7-run-the-full-stack-with-aspire)
8. [Verify Services Are Running](#8-verify-services-are-running)
9. [Run Tests](#9-run-tests)
10. [Utility Scripts Reference](#10-utility-scripts-reference)
11. [Troubleshooting](#11-troubleshooting)

---

## 1. Prerequisites

Install every tool below before continuing. Confirm each with the verification command.

### 1.1 .NET 10 SDK

Download from: https://dotnet.microsoft.com/download/dotnet/10.0

```powershell
# Verify
dotnet --version
# Expected: 10.x.x
```

### 1.2 .NET Aspire Workload

```powershell
dotnet workload install aspire
```

```powershell
# Verify
dotnet workload list
# Expected: 'aspire' appears in the list
```

### 1.3 EF Core CLI Tools

```powershell
dotnet tool install --global dotnet-ef
```

```powershell
# Verify
dotnet ef --version
# Expected: Entity Framework Core .NET Command-line Tools x.x.x
```

### 1.4 Docker Desktop

Download from: https://www.docker.com/products/docker-desktop

```powershell
# Verify Docker daemon is running
docker info
# Verify version
docker --version
# Expected: Docker version 24+
```

> **Important:** Docker Desktop must be **running** every time you start the application. All backing services (PostgreSQL, RabbitMQ, Redis, EventStoreDB, Elasticsearch, etc.) are spun up as containers automatically by Aspire.

### 1.5 (Optional) Visual Studio 2022 / Rider / VS Code

- **Visual Studio 2022** v17.12+ with the **Aspire** workload selected in the installer.
- **JetBrains Rider** 2024.3+ — install the [.NET Aspire plugin](https://plugins.jetbrains.com/plugin/23289).
- **VS Code** — install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension.

---

## 2. Clone & Verify the Repository

```powershell
# Clone (adjust URL to your remote)
git clone <repo-url> AI
cd AI

# Confirm solution file is present
ls AI.sln
```

Expected structure:
```
AI/
├── AI.sln
├── Docs/
├── Src/
│   ├── Api/          ← AI.Api entry point
│   ├── Aspire/       ← AppHost & ServiceDefaults
│   ├── Common/       ← Shared building blocks
│   ├── Modules/      ← 22 business modules
│   └── Tests/
└── scripts/          ← Migration PowerShell helpers
```

---

## 3. Install .NET Tooling

Run from the **repository root** (`d:\dev\_projects\AI`):

```powershell
# Restore all NuGet packages for the entire solution
dotnet restore AI.sln
```

Expected output: `Restore succeeded` with no errors.

---

## 4. Verify Docker

Make sure Docker Desktop is open and the daemon is responsive before proceeding:

```powershell
docker info
docker ps
```

Both commands should return without error. If `docker info` fails, start Docker Desktop and wait for it to finish initialising.

---

## 5. Restore & Build the Solution

```powershell
# From repo root
dotnet build AI.sln
```

This builds all 22 modules, Common, Api, Aspire AppHost, and ServiceDefaults in one pass.

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

> If you get NuGet feed errors, check your internet connection or corporate proxy settings.

---

## 6. Database Migrations

> **Note:** When running via Aspire (step 7), the API starts after all databases are healthy. If you need to apply migrations manually (e.g. first-time setup without Aspire, or after adding a new migration), use the scripts below.

All scripts must be run from the **repository root**.

### 6.1 Add a New Migration (all modules at once)

```powershell
# Windows PowerShell
.\scripts\Add-Migrations.ps1 -MigrationName "Initial"
```

```bash
# Linux / macOS / Git Bash
bash scripts/add-migration.sh Initial
```

The script auto-discovers every `*Context.cs` file under `Src/Modules/**/Data/` and runs `dotnet ef migrations add` for each context using `AI.Api` as the startup project.

### 6.2 Apply Migrations to All Module Databases

```powershell
# Windows PowerShell
.\scripts\Update-Databases.ps1
```

```bash
# Linux / macOS / Git Bash
bash scripts/update-db.sh
```

> **Prerequisite:** A PostgreSQL instance must already be running (e.g., the Aspire-managed container on port `5432`) before running `Update-Databases.ps1`.

### 6.3 Manual Migration for a Single Module

If you only changed one module's schema:

```powershell
dotnet ef migrations add <MigrationName> `
    --project Src/Modules/<ModuleName>/<ModuleName>.Infrastructure/<ModuleName>.Infrastructure.csproj `
    --startup-project Src/Api/AI.Api/AI.Api.csproj `
    --context <ModuleName>DbContext `
    --output-dir Data/Migrations
```

```powershell
dotnet ef database update `
    --project Src/Modules/<ModuleName>/<ModuleName>.Infrastructure/<ModuleName>.Infrastructure.csproj `
    --startup-project Src/Api/AI.Api/AI.Api.csproj `
    --context <ModuleName>DbContext
```

---

## 7. Run the Full Stack with Aspire

This is the **single command** that starts everything: PostgreSQL (22 databases), Redis, RabbitMQ, EventStoreDB, Elasticsearch, Kibana, Jaeger, Zipkin, Prometheus, Grafana, Loki, Tempo, OpenTelemetry Collector, and the `AI.Api` itself.

```powershell
# From repo root — recommended
dotnet run --project Src/Aspire/AppHost/AppHost.csproj
```

Or navigate into the AppHost folder first:

```powershell
cd Src/Aspire/AppHost
dotnet run
```

### What Aspire Does Automatically

| Step | Detail |
|------|--------|
| Pulls Docker images | `postgres:latest`, `redis:latest`, `rabbitmq:management`, `eventstore/eventstore`, `elasticsearch:8.17.0`, `kibana:8.17.0`, `prom/prometheus`, `grafana/grafana`, `jaegertracing/all-in-one`, `openzipkin/zipkin`, `grafana/tempo`, `grafana/loki`, `otel/opentelemetry-collector-contrib` |
| Creates 22 Postgres databases | One per module (identity, ai-orchestration, chat-bot, user, …) |
| Waits for health checks | API does not start until all databases + EventStoreDB + RabbitMQ are healthy |
| Launches AI.Api | HTTP on `localhost:3001`, HTTPS on `localhost:3000` |
| Launches Aspire Dashboard | URL printed in the terminal output |

> **First run** will download ~3–5 GB of Docker images. Subsequent runs start in seconds because images are cached.

### Aspire Dashboard

After startup, the terminal will print a line similar to:
```
Login to the dashboard at: http://localhost:15232/login?t=<token>
```

Open that URL to see real-time container status, structured logs, and distributed traces for every service.

---

## 8. Verify Services Are Running

Once Aspire reports all resources as **Running**, verify the key endpoints:

| Service | URL | Credentials |
|---------|-----|-------------|
| **AI API (HTTP)** | http://localhost:3001 | — |
| **AI API (HTTPS)** | https://localhost:3000 | — |
| **Aspire Dashboard** | URL in terminal output | Token in terminal |
| **RabbitMQ Management** | http://localhost:15672 | `guest` / `guest` |
| **EventStoreDB UI** | http://localhost:2113 | — (insecure mode) |
| **Grafana** | http://localhost:3000 | `admin` / `admin` |
| **Prometheus** | http://localhost:9090 | — |
| **Jaeger UI** | http://localhost:16686 | — |
| **Zipkin UI** | http://localhost:9411 | — |
| **Kibana** | http://localhost:5601 | — |
| **Elasticsearch** | http://localhost:9200 | — (security disabled) |

Quick smoke-test the API:

```powershell
# Health check (if HealthOptions.Enabled = true in appsettings.json)
curl http://localhost:3001/health

# Or just confirm it responds
curl http://localhost:3001/
```

---

## 9. Run Tests

Integration tests use **Testcontainers** — Docker must be running.

```powershell
# Run all tests from solution root
dotnet test AI.sln

# Run with detailed output
dotnet test AI.sln --logger "console;verbosity=detailed"

# Run a specific test project
dotnet test Src/Tests/<TestProjectName>/<TestProjectName>.csproj

# Run with coverage (requires coverlet)
dotnet test AI.sln --collect:"XPlat Code Coverage"
```

---

## 10. Utility Scripts Reference

All scripts are in the `scripts/` folder and must be run from the **repository root**.

| Script | Platform | Purpose |
|--------|----------|---------|
| `Add-Migrations.ps1` | PowerShell (Windows) | Adds a named migration to **all** module DbContexts simultaneously |
| `Update-Databases.ps1` | PowerShell (Windows) | Applies all pending migrations to **all** module databases |
| `add-migration.sh` | Bash (Linux/macOS) | Same as `Add-Migrations.ps1` |
| `update-db.sh` | Bash (Linux/macOS) | Same as `Update-Databases.ps1` |

### Full Script Usage Examples

```powershell
# Add migration named "AddUserPreferences" across all modules
.\scripts\Add-Migrations.ps1 -MigrationName "AddUserPreferences"

# Apply all pending migrations
.\scripts\Update-Databases.ps1
```

Both scripts auto-discover all `.csproj` files under `Src/Modules/` that have a `Data/` subfolder containing a `*Context.cs` file, then run the appropriate EF Core command for each.

---

## 11. Troubleshooting

### Docker containers fail to start

```powershell
# Check Docker daemon
docker info

# View running containers
docker ps -a

# View logs for a specific container (name comes from Aspire resource name)
docker logs postgres
docker logs rabbitmq
```

### Port already in use

If you have existing local services on ports `5432`, `6379`, `5672`, etc., stop them before running Aspire:

```powershell
# Check what is using port 5432
netstat -aon | findstr :5432
# Kill the process (replace PID)
Stop-Process -Id <PID> -Force
```

### NuGet restore errors

```powershell
# Clear the NuGet cache
dotnet nuget locals all --clear

# Re-restore
dotnet restore AI.sln
```

### EF Core tool not found

```powershell
dotnet tool install --global dotnet-ef
# Or update if already installed
dotnet tool update --global dotnet-ef
```

### Aspire workload missing

```powershell
dotnet workload install aspire
# Or update
dotnet workload update
```

### `HTTPS certificate` errors on first run

```powershell
dotnet dev-certs https --trust
```

### Database connection refused

Ensure the Aspire-managed PostgreSQL container is healthy before running migration scripts:

```powershell
docker ps --filter name=postgres
# Status should be: Up (healthy)
```

---

## Quick-Start Cheat Sheet

```powershell
# ── One-time setup ──────────────────────────────────────────
dotnet workload install aspire
dotnet tool install --global dotnet-ef
dotnet dev-certs https --trust

# ── Every session ───────────────────────────────────────────
# 1. Ensure Docker Desktop is running
# 2. From repo root:
dotnet restore AI.sln
dotnet build AI.sln
dotnet run --project Src/Aspire/AppHost/AppHost.csproj

# ── After schema changes ─────────────────────────────────────
.\scripts\Add-Migrations.ps1 -MigrationName "YourMigrationName"
.\scripts\Update-Databases.ps1
```
