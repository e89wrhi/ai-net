# 🚀 Aspire Build & Run Guide

.NET Aspire is the primary development environment for AI-Net. It simplifies local development by managing service orchestration, health checks, and a central dashboard.

---

## 🛠️ Prerequisites

Running with Aspire requires the .NET SDK and the Aspire workload.

| Tool | Required for | Install |
|---|---|---|
| .NET 10 SDK | Core Runtime | [Download](https://dotnet.microsoft.com/download/dotnet/10.0) |
| Docker Desktop 24+ | Service containers | [Download](https://www.docker.com/products/docker-desktop) |
| Aspire workload | Orchestration | `dotnet workload install aspire` |
| EF Core CLI | Database Migrations | `dotnet tool install --global dotnet-ef` |

```powershell
# Verify installation
dotnet --version          # 10.x.x
docker --version          # 24+
dotnet workload list      # confirm 'aspire' appears
```

---

## 🏃 Getting Started

```powershell
# 1. Restore dependencies
dotnet restore AI.sln

# 2. Run the AppHost
dotnet run --project Src/Aspire/AppHost/AppHost.csproj
```

That's it. Aspire pulls and starts everything:
- **PostgreSQL** (Database)
- **RabbitMQ** (Messaging)
- **EventStoreDB** (Events)
- **AI.Api** (Application)

Open the **Aspire Dashboard** URL printed in your terminal (usually `http://localhost:18888`) to view logs, metrics, and traces.

---

## 🔄 Development Workflow

### Applying Migrations
When you change your domain entities, you need to sync the database.

```powershell
# Add new migration to all modules
.\scripts\Add-Migrations.ps1 -MigrationName "MyChange"

# Apply all pending migrations
.\scripts\Update-Databases.ps1
```

### Manual EF Core Commands
If you need to target a specific module:
```powershell
dotnet ef migrations add MyChange `
  --project Src/Modules/<Name>/<Name>.Infrastructure/<Name>.Infrastructure.csproj `
  --startup-project Src/Api/AI.Api/AI.Api.csproj `
  --context <Name>DbContext `
  --output-dir Data/Migrations
```

---

## 📡 Service Endpoints

| Service | URL | Notes |
|---|---|---|
| **AI API** | http://localhost:3001 | Swagger available at `/swagger` |
| **Aspire Dashboard** | *See Terminal Output* | Logs, Metrics, Traces |
| **RabbitMQ UI** | http://localhost:15672 | `guest`/`guest` |
| **EventStoreDB** | http://localhost:2113 | Stream browser |

---

## 🧪 Running Tests

```powershell
dotnet test AI.sln
dotnet test AI.sln --logger "console;verbosity=detailed"
```

> [!NOTE]
> Tests use **Testcontainers**, which automatically manages its own Docker containers. You don't need to have the main application running to run tests.

---

## 🔍 Troubleshooting

| Problem | Fix |
|---|---|
| `docker info` fails | Ensure Docker Desktop is running and initialized. |
| Port 18888 busy | Change dashboard port in `AppHost/appsettings.json`. |
| NuGet errors | Run `dotnet nuget locals all --clear` then `dotnet restore`. |
| HTTPS cert errors | Run `dotnet dev-certs https --trust`. |
| Service won't start | Check logs in the Aspire Dashboard for specific error messages. |
