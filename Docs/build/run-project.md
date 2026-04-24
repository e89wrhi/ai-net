# Operations Guide

> For setup and first-time builds see [**Aspire Guide**](./build-aspire-project.md) or [**Docker Guide**](./build-docker-project.md).

---

## Entrypoint Scripts

### Linux / macOS / Git Bash

```bash
./scripts/entrypoint.sh                          # clean → build → run
./scripts/entrypoint.sh --skip-clean             # skip clean (faster)
./scripts/entrypoint.sh --run-migrations         # apply DB migrations before run
./scripts/entrypoint.sh --environment Staging    # set ASP.NET Core environment
```

### Windows PowerShell

```powershell
.\scripts\entrypoint.ps1
.\scripts\entrypoint.ps1 -SkipClean
.\scripts\entrypoint.ps1 -RunMigrations
.\scripts\entrypoint.ps1 -SkipClean -RunMigrations -Environment Staging
```

### Inside Docker (container mode)

The same `entrypoint.sh` is used inside the container. It:
1. Waits for PostgreSQL (`pg_isready`, 60 s timeout)
2. Waits for RabbitMQ (TCP check, 60 s timeout)
3. Optionally applies migrations if `RUN_MIGRATIONS=true`
4. Starts `dotnet AI.Api.dll`

```bash
# Trigger migrations inside the container
docker compose run --rm -e RUN_MIGRATIONS=true ai-api
```

---

## Docker Compose — Quick Reference

```bash
# First time
cp .env.example .env           # fill in passwords
docker compose up --build      # build + start all services

# Day to day
docker compose up -d           # start in background
docker compose down            # stop (keep data)
docker compose down -v         # stop + wipe all data volumes

# Rebuild API only
docker compose up --build ai-api

# Logs
docker compose logs -f              # all services
docker compose logs -f ai-api       # API only

# Status
docker compose ps

# Run a command inside a container
docker compose exec postgres psql -U postgres -l
docker compose exec rabbitmq rabbitmqctl list_queues
```

### .env Variables

| Variable | Default | Purpose |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | App environment |
| `POSTGRES_USER` | `postgres` | DB user |
| `POSTGRES_PASSWORD` | `changeme` | DB password |
| `RABBITMQ_USER` | `guest` | Broker user |
| `RABBITMQ_PASSWORD` | `changeme` | Broker password |
| `API_HTTP_PORT` | `8080` | API HTTP port |
| `API_HTTPS_PORT` | `8443` | API HTTPS port |
| `JWT_AUTHORITY` | — | JWT issuer URL |
| `JWT_AUDIENCE` | — | JWT audience |

---

## CI/CD (GitHub Actions)

| Workflow | Trigger | What it does |
|---|---|---|
| `ci.yml` | Every push / PR → `main`, `develop` | Format check → build → test |
| `cd.yml` | Push → `main` or version tag `v*.*.*` | Verify → build Docker image → push to GHCR |
| `security.yml` | Push / PR + weekly Monday | NuGet audit + CodeQL + Trivy image scan |

**GHCR image tags:**

| Event | Tags |
|---|---|
| Push → `main` | `:latest` `:main` `:sha-<7>` |
| Tag `v1.2.3` | `:1.2.3` `:1.2` `:1` `:sha-<7>` |

```bash
docker pull ghcr.io/<org>/ai-net:latest
```

No secrets needed — workflows use the built-in `GITHUB_TOKEN`.

---

## Configuration Overrides

ASP.NET Core uses `__` as a separator for environment variable overrides:

```bash
PostgresOptions__ConnectionString__Identity=Host=db;Database=ai_identity;Username=app;Password=secret
EventStoreDB__ConnectionString=esdb://esdb-host:2113?tls=true
RabbitMQ__Host=rabbitmq-prod
Jwt__Authority=https://auth.example.com
Jwt__Audience=ai-net-api
```

---

## Security

- Container runs as non-root user (`appuser`)
- No secrets baked into the image — injected via `.env` or environment variables
- `.env` is git-ignored (only `.env.example` is committed)
- `security.yml` scans NuGet deps + Docker image weekly
