# 🐳 Docker Compose Build & Run Guide

Docker Compose is used for production-like environments, staging, and CI/CD validation.

---

## 🛠️ Prerequisites

| Tool | Required for | Install |
|---|---|---|
| Docker Desktop 24+ | Service containers | [Download](https://www.docker.com/products/docker-desktop) |
| .NET SDK | Compiling the app locally | [Download](https://dotnet.microsoft.com/download) |

```bash
# Verify installation
docker --version          # 24+
docker compose version    # V2
dotnet --version
```

---

## 🏃 Getting Started

### 1. Configuration
Create your local environment file from the template:
```bash
cp .env.example .env
```

### 2. Publish Project Locally
Since the API Dockerfile is optimized for runtime only, you must first publish the built binaries locally so Docker can copy them:
```bash
dotnet publish Src/Api/AI.Api/AI.Api.csproj -c Release -o ./publish
```

### 3. Launch Services
Start all services in the background:
```bash
docker compose up -d --build
```

### 4. Verify Health
Check that all containers are healthy:
```bash
docker compose ps
```

---

## 🔧 Useful Commands

| Action | Command |
|---|---|
| **Stop everything** | `docker compose down` |
| **Reset Data** | `docker compose down -v` (wipes volumes) |
| **Rebuild API** | `docker compose up -d --build ai-api` |
| **View Logs** | `docker compose logs -f ai-api` |
| **Check Stats** | `docker stats` |

---

## 🔄 Database Migrations

In Docker mode, the database and application are isolated. To apply migrations:

1. **Ensure Postgres is running:**
   ```bash
   docker compose up postgres -d
   ```

2. **Run the update script:**
   - **Windows:** `.\scripts\Update-Databases.ps1`
   - **Linux/macOS:** `bash scripts/update-db.sh`

---

## 📡 Service Endpoints

| Service | URL | Notes |
|---|---|---|
| **AI API** | http://localhost:8080 | Production entry point |
| **API Health** | http://localhost:8080/health | Readiness check |
| **RabbitMQ UI** | http://localhost:15672 | `guest`/`guest` |
| **EventStoreDB** | http://localhost:2113 | admin/changeit |

---

## 🔍 Troubleshooting

| Problem | Fix |
|---|---|
| `"/publish": not found` error | You did not run `dotnet publish` locally. Run step 2 above. |
| `failed to do request... EOF` | Network timeout downloading images. Rerun `docker compose up -d --build`. |
| Container exits immediately | Check logs: `docker compose logs [service-name]` |
| DB connection refused | Ensure `postgres` container is `healthy` in `docker compose ps` |
| Port already in use | Run `docker compose down` to free up ports from previous runs |
| Changes not reflected | Rebuild with `docker compose up --build` |

> [!TIP]
> If you are doing active code changes, consider using **.NET Aspire** instead for a faster feedback loop. See [Aspire Guide](./build-aspire-project.md).
