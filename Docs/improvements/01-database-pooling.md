# 🗄️ Database Sprawl & Connection Pool Exhaustion

## Overview
In the AI-Net architecture, we have adopted a strict **Modular Monolith** pattern. This means there are 20+ independent modules (`ChatBot`, `CodeGen`, `Identity`, etc.). To enforce physical data boundaries, each module instantiates its own Entity Framework `DbContext`.

While this enforces excellent Domain-Driven Design (DDD), it introduces a severe production risk: **Connection Pooling Exhaustion**.

## The Core Problem
By default, Entity Framework Core manages a connection pool for every `DbContext` type. 
If each of your 20 modules holds onto 10-20 connections minimum in its pool, a single instance of your API reserves 200-400 connections to the Postgres database. 
- If you scale your API to 5 instances in a Kubernetes cluster, you'll suddenly demand 2,000+ connections from Postgres.
- Most default Postgres installations max out at 100 connections.
- Your application will crash with `TimeoutException: Timeout expired. The timeout period elapsed prior to obtaining a connection from the pool.`

## The Solution: PgBouncer

To solve this, we must decouple the EF Core connection pool from the actual, physical Postgres hardware connections. We do this by placing **PgBouncer** (a lightweight connection pooler) in front of Postgres.

### Implementation Steps

#### 1. Add PgBouncer to Docker Compose
Modify your `docker-compose.yml` to include PgBouncer:

```yaml
  pgbouncer:
    image: edoburu/pgbouncer:latest
    environment:
      - DATABASE_URL=postgres://${POSTGRES_USER}:${POSTGRES_PASSWORD}@postgres:5432/postgres
      - MAX_CLIENT_CONN=1000  # EF Core can open 1000 connections to PgBouncer
      - DEFAULT_POOL_SIZE=50  # PgBouncer only opens 50 connections to the actual DB
      - POOL_MODE=transaction # CRITICAL: Releases connection after every transaction
    ports:
      - "6432:6432"
    depends_on:
      - postgres
```

#### 2. Update Configuration Settings
Change the `appsettings.json` connection strings so that all modules connect to port `6432` rather than `5432`.

```json
"PostgresOptions": {
    "ConnectionString": {
        "AiOrchestration": "Host=pgbouncer;Port=6432;Database=ai_orchestration;Username=postgres;Password=postgres;Max Pool Size=20",
        "ChatBot": "Host=pgbouncer;Port=6432;Database=ai_chatbot;Username=postgres;Password=postgres;Max Pool Size=20"
    }
}
```

#### 3. Prepare EF Core for Transaction Pooling
Because `PgBouncer` is set to `transaction` mode (meaning it forcefully takes the Postgres connection back the millisecond the `COMMIT` fires), EF Core prepared statements can sometimes break.
In your Common `EFCore` module configuration, disable prepared statements if errors arise:

```csharp
services.AddDbContext<CodeDbContext>(options =>
    options.UseNpgsql(connString, npgsqlOptions =>
    {
        // PgBouncer compatibility
        npgsqlOptions.NoServerCaches(); 
    }));
```

## Outcome
With PgBouncer, your 20 `DbContexts` can open hundreds of connections to PgBouncer without crashing the system. PgBouncer efficiently funnels them all through heavily optimized, shared transactions to the physical database, securing massive scalability.
