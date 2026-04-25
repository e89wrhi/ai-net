# 📚 Project Documentation Hub

Welcome to the central documentation repository for the **AI Platform**. This project is built as a high-performance, modular monolith using .NET 9, Aspire, and modern cloud-native patterns.

---

## 🏛️ [Architecture](./architecture)
Understand the "why" and "how" behind our technical decisions.
- **[System Structure](./architecture/000-system-structure.md)**: High-level overview of the Modular Monolith logic and components.
- **[Database Strategy](./architecture/001-arch-db.md)**: Handling multi-tenant and multi-module persistence.
- **[Image Processing](./architecture/002-arch-image.md)**: Scalable pipelines for AI-generated assets.
- **[Search Engine](./architecture/003-arch-search.md)**: Integration of vector search and traditional indexing.
- **[Activity Feed](./architecture/004-arch-feed.md)**: Event-driven notification and newsfeed logic.

## 🛠️ [Development Guide](./guide)
Everything you need to get the system running locally.
- **[Quick Start](./guide/getting-started.md)**: 5 minutes to `dotnet run`.
- **[Aspire Workflow](./guide/aspire-dashboard.md)**: Using the diagnostic dashboard for local dev.
- **[Contribution Rules](./guide/contribution.md)**: Coding standards and PR process.

## 🧩 [Modules](./modules)
Detailed documentation for each of the 12+ functional modules.
- **Identity**: AuthN/AuthZ using Duende IdentityServer.
- **ChatBot**: LLM orchestration and session management.
- **CodeGen**: AI-assisted programming pipelines.
- **[View All Modules...](./modules)**

## 🧪 [Testing Strategy](./test)
How we ensure excellence and prevent regressions.
- **[Unit Testing](./test/unit-testing.md)**: Patterns for testing domain logic.
- **[Integration Testing](./test/new-unit-test.md)**: Using TestContainers for real infra validation.

## 🚀 [Deployment & Build](./build)
- **Build Guides**: [**🚀 Aspire (Dev)**](./build/build-aspire-project.md) \| [**🐳 Docker (Prod-like)**](./build/build-docker-project.md)
- **[Production Deployment](./build/production.md)**: Best practices and infrastructure scaling requirements.
- **CI/CD Pipelines**: GitHub Actions workflow details.
- **Dockerization**: Containerization strategy and registry management.

---

## 🗺️ Project Roadmap
1. [ ] Complete XML documentation for all Building Blocks.
2. [ ] Replace Simulated AI Clients with production-ready providers.
3. [ ] Implement comprehensive integration test suite.
4. [ ] Standardize auto-discovery for module migrations.

> [!TIP]
> Use the **.NET Aspire Dashboard** during development to view real-time traces and logs across all these modules simultaneously.
