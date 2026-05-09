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
Start all services in the background. The API service uses the `docker` profile, so pass `--profile docker`:
```bash
docker compose --profile docker up -d --build
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
| **Rebuild API** | `docker compose --profile docker up -d --build ai-api` |
| **View Logs** | `docker compose logs -f ai-api` |
| **Check Stats** | `docker stats` |

---

## 🔄 Database Migrations

The runtime Docker image does **not** include the .NET SDK or `dotnet-ef`, so migrations cannot be run from inside the container. Apply them from your host machine before starting the API.

1. **Start Postgres first:**
   ```bash
   docker compose up -d postgres
   ```

2. **Run the migration script from the repo root:**
   - **Windows:** `.\scripts\Update-Databases.ps1`
   - **Linux/macOS:** `bash scripts/update-db.sh`

> [!NOTE]
> The migration scripts connect to `localhost:${POSTGRES_PORT:-5432}`. Make sure that port is mapped in your `.env` (default: `5432`).

---

## 📡 Service Endpoints

| Service | URL | Notes |
|---|---|---|
| **AI API (HTTP)** | http://localhost:8080 | `API_HTTP_PORT` in `.env` |
| **AI API (HTTPS)** | https://localhost:8443 | `API_HTTPS_PORT` → container port `8081` |
| **API Health** | http://localhost:8080/health | Readiness check |
| **RabbitMQ UI** | http://localhost:15672 | `RABBITMQ_USER`/`RABBITMQ_PASSWORD` from `.env` |
| **EventStoreDB** | http://localhost:2113 | `ESDB_HTTP_PORT` in `.env`; default creds: `admin`/`changeit` |

---

## 🔍 Troubleshooting

| Problem | Fix |
|---|---|
| `"/publish": not found` error | You did not run `dotnet publish` locally. Run step 2 above. |
| `ai-api` service not starting | Add `--profile docker` to your `docker compose` command. |
| `failed to do request... EOF` | Network timeout downloading images. Rerun `docker compose --profile docker up -d --build`. |
| Container exits immediately | Check logs: `docker compose logs [service-name]` |
| DB connection refused | Ensure `postgres` container is `healthy` in `docker compose ps` |
| Port already in use | Run `docker compose down` to free up ports from previous runs |
| Changes not reflected | Rebuild with `docker compose --profile docker up --build` |
| Migrations not applied | SDK not in runtime image — run `Update-Databases.ps1` / `update-db.sh` from your host. |

> [!TIP]
> If you are doing active code changes, consider using **.NET Aspire** instead for a faster feedback loop. See [Aspire Guide](./build-aspire-project.md).
