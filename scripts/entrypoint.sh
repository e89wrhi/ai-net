#!/bin/bash
# AI-Net Entrypoint Script
# Handles database wait, optional migrations, and application startup.
# Works in both local Development/Source mode and Docker Container mode.

set -euo pipefail

# ─── Helpers ────────────────────────────────────────────────────────────────
log()  { echo -e "\033[36m[entrypoint]\033[0m $*"; }
warn() { echo -e "\033[33m[entrypoint] ⚠️  $*\033[0m"; }
err()  { echo -e "\033[31m[entrypoint] ❌ $*\033[0m" >&2; }

# Resolve script directory robustly (works when sourced or executed)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

# ─── Development / Source Mode ───────────────────────────────────────────────
# Detected when the solution file exists in the working directory.
if [ -f "$ROOT_DIR/AI.sln" ] || [ -f "AI.sln" ]; then
    log "Running in Development / Source Mode..."

    SKIP_CLEAN=false
    RUN_MIGRATIONS=false

    while [[ $# -gt 0 ]]; do
        case $1 in
            --skip-clean)      SKIP_CLEAN=true;      shift ;;
            --run-migrations)  RUN_MIGRATIONS=true;  shift ;;
            --environment)     export ASPNETCORE_ENVIRONMENT="${2:-Development}"; shift 2 ;;
            *) warn "Unknown argument: $1"; shift ;;
        esac
    done

    if [ "$SKIP_CLEAN" = false ]; then
        log "Step 1: Cleaning..."
        bash "$SCRIPT_DIR/clean.sh"
    else
        log "Step 1: Skipping clean (--skip-clean)"
    fi

    log "Step 2: Restoring .NET tools..."
    dotnet tool restore

    log "Step 3: Building..."
    bash "$SCRIPT_DIR/build.sh"

    if [ "$RUN_MIGRATIONS" = true ]; then
        log "Step 4: Applying EF Core migrations for all modules..."
        bash "$SCRIPT_DIR/update-db.sh"
    else
        log "Step 4: Skipping migrations (pass --run-migrations to apply)"
    fi

    log "Step 5: Starting application..."
    exec dotnet run --project "$ROOT_DIR/Src/Api/AI.Api/AI.Api.csproj" --no-build
fi

# ─── Container Mode ──────────────────────────────────────────────────────────
log "Running in Container Mode..."

# Graceful shutdown support
trap 'log "Shutting down..."; kill "$APP_PID" 2>/dev/null; wait "$APP_PID" 2>/dev/null' SIGTERM SIGINT

# --- Wait for PostgreSQL -------------------------------------------------------
PG_HOST="${POSTGRES_HOST:-postgres}"
PG_PORT="${POSTGRES_PORT:-5432}"
PG_USER="${POSTGRES_USER:-postgres}"
PG_MAX_WAIT="${PG_MAX_WAIT:-60}"

log "Waiting for PostgreSQL at $PG_HOST:$PG_PORT (timeout: ${PG_MAX_WAIT}s)..."
WAITED=0
until pg_isready -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -q 2>/dev/null; do
    if [ "$WAITED" -ge "$PG_MAX_WAIT" ]; then
        err "PostgreSQL did not become ready within ${PG_MAX_WAIT}s. Aborting."
        exit 1
    fi
    sleep 2
    WAITED=$((WAITED + 2))
done
log "✅ PostgreSQL is ready."

# --- Wait for RabbitMQ --------------------------------------------------------
RABBIT_HOST="${RABBITMQ_HOST:-rabbitmq}"
RABBIT_PORT="${RABBITMQ_PORT:-5672}"
RABBIT_MAX_WAIT="${RABBIT_MAX_WAIT:-60}"

log "Waiting for RabbitMQ at $RABBIT_HOST:$RABBIT_PORT (timeout: ${RABBIT_MAX_WAIT}s)..."
WAITED=0
until </dev/tcp/"$RABBIT_HOST"/"$RABBIT_PORT" 2>/dev/null; do
    if [ "$WAITED" -ge "$RABBIT_MAX_WAIT" ]; then
        warn "RabbitMQ not ready after ${RABBIT_MAX_WAIT}s — continuing anyway."
        break
    fi
    sleep 2
    WAITED=$((WAITED + 2))
done
log "✅ RabbitMQ is ready (or timed out — app will retry)."

# --- EF Core Migrations -------------------------------------------------------
# In production containers the SDK is NOT present. Migrations should be applied
# by a dedicated migrator image/job. If RUN_MIGRATIONS=true is explicitly set
# and dotnet-ef is available (e.g. in a migrator-stage image), we run them.
if [ "${RUN_MIGRATIONS:-false}" = "true" ]; then
    if command -v dotnet-ef &>/dev/null || dotnet tool list -g 2>/dev/null | grep -q "dotnet-ef"; then
        log "Applying EF Core migrations..."
        bash "$SCRIPT_DIR/update-db.sh"
    else
        warn "dotnet-ef not found in this image. Skipping migrations."
        warn "Use a migrator stage image or run 'dotnet ef database update' separately."
    fi
fi

# --- Start the API ------------------------------------------------------------
log "Starting AI.Api..."
exec dotnet AI.Api.dll
