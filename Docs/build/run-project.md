# Operations Guide: Build, Production & CI/CD

This document outlines how to build the project, prepare it for production, and setup a CI/CD pipeline.

---

## 🏗️ 1. Building the Project

### Local Development
The project uses **.NET Aspire** for orchestration, which handles all dependencies (Postgres, RabbitMQ, etc.) via Docker.

1.  **Build Solution**:
    ```bash
    dotnet build AI.sln
    ```
2.  **Run with Aspire**:
    ```bash
    dotnet run --project Src/Aspire/AppHost
    ```

### Running Tests
The project uses `Testcontainers` for integration tests. Ensure Docker is running.
```bash
dotnet test
```

---

## 🚀 2. Production Readiness

### Containerization
Each module is designed to be containerized. The `AI.Api` is the primary entry point for the monolithic deployment, but modules can be extracted into individual microservices.

**Draft Dockerfile for `AI.Api`**:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Src/Api/AI.Api/AI.Api.csproj", "Src/Api/AI.Api/"]
... # Copy all other project files
RUN dotnet restore "Src/Api/AI.Api/AI.Api.csproj"
COPY . .
RUN dotnet build "Src/Api/AI.Api/AI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Src/Api/AI.Api/AI.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.Api.dll"]
```

### Aspire Deployment
For production, .NET Aspire can be deployed to:
- **Azure Container Apps** (via `azd deploy`)
- **Kubernetes** (via Aspir8 tool)

---

## 🤖 3. CI/CD Pipeline (GitHub Actions)

Since this is a .NET project, a standard GitHub Action should include:

### Recommended Workflow Steps:
1.  **Checkout Code**
2.  **Setup .NET SDK 9.0**
3.  **Restore Dependencies**: `dotnet restore`
4.  **Lint/Code Format Check**: `dotnet format --verify-no-changes`
5.  **Build**: `dotnet build --no-restore -c Release`
6.  **Test**: `dotnet test --no-build -c Release`
7.  **Publish Artifacts**: Optional, if deploying to a registry.

### Sample Workflow File (`.github/workflows/main.yml`):
```yaml
name: .NET CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    services:
      # Start Docker services if needed for tests, or use Testcontainers
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 9.0.x
    - name: Restore
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore -c Release
    - name: Test
      run: dotnet test --no-build -c Release
```

---

## 🛡️ 4. Security & Monitoring
## ⚙️ 5. Configuration & Environment Variables

The application is configured via `appsettings.json` and overrides.

### Key Configuration Sections:
- **`PostgresOptions`**: Individual connection strings for `Note`, `Identity`, `Order`, and `Summary` modules.
- **`EventStoreOptions`**: Connection to EventStoreDB for event sourcing.
- **`ObservabilityOptions`**: Controls where traces are sent (OTLP, Zipkin, Jaeger).
- **`Jwt`**: Authority and Audience for authentication.

### Production Overrides:
In production, use **Environment Variables** (standard ASP.NET Core practice):
- `PostgresOptions__ConnectionString__Note`
- `EventStoreOptions__ConnectionString`
- `Jwt__Authority`
