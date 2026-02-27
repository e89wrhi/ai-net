# 🏗️ Build Guide

This document outlines the build process for the AI Platform. 

## Local Build Process

The project is structured as a .NET 9 Modular Monolith using Aspire for orchestration. 

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for TestContainers and local backing services)
- Visual Studio 2022 (v17.10+) or JetBrains Rider or VS Code

### Build Commands

To build the entire solution:
```bash
dotnet build AI.sln
```

### Aspire Orchestration

We use .NET Aspire to manage the complex graph of dependencies (databases, brokers, cache). The `AppHost` project is the entry point for local development.

```bash
cd Src/Aspire/AppHost
dotnet run
```

This will automatically spin up all required containers, apply migrations (if configured), and launch the .NET API application along with the Aspire Dashboard.

### Container Builds

Each module is designed to be executable as a standalone service if needed, although they typically run together in the Monolith API. 

To build a production Docker image for the main API:
```bash
docker build -t ai-platform-api -f Src/Api/Dockerfile .
```
