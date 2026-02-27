# 🏛️ System Structure

The AI Platform leverages a **Modular Monolith** architecture. This guide explores how the components are organized and how to navigate the codebase to truly understand the system.

## High-Level Architecture

The system is fundamentally separated into three layers:
- **API Boundary (`Src/Api`)**: A single unified REST and gRPC API that aggregates the underlying modules. It acts as the Host process.
- **Modules (`Src/Modules`)**: 20+ independent, vertically-sliced business domains (e.g., `ChatBot`, `Identity`, `ImageGen`). Each module models exactly one bounded context.
- **Building Blocks (`Src/Common`)**: Shared infrastructure, abstractions, DDD patterns, and implementations used across modules.

Additionally, the **`Src/Aspire`** orchestrator automatically wires up the application and its background services (Postgres, Redis, RabbitMQ) during local development.

## Anatomy of a Module

Each module strictly enforces Domain-Driven Design (DDD) and Clean Architecture within its own boundary. Modules are highly isolated and share *nothing* except basic Common primitives.

```text
Src/Modules/[ModuleName]/
├── Features/        # Use Cases using CQRS (Commands, Queries, Handlers, Validators via MediatR)
├── Domain/          # Core abstractions, Entities, Value Objects, Domain Events
├── Data/            # EF Core DbContext, Configurations, Entity Migrations
├── Endpoints/       # Minimal API route definitions (mapped gracefully at API layer)
├── Extensions/      # Dependency injection composition root for this module
```

## Inter-Module Dependency Rules

To maintain absolute decoupling, **modules never reference each other directly**. 
All cross-module communication is event-driven or delegated:
1. **Asynchronous Messaging**: Publishing Integration Events via MassTransit (RabbitMQ) allows modules to react to events occurring elsewhere.
2. **In-Process Queries**: Using MediatR cross-module contracts, if synchronous dependencies are absolutely forced (use sparingly to avoid coupling).
3. **gRPC**: Occasionally used when high-performance remote-like procedures are needed internally.

## Consistency and Transactions

The system embraces Eventual Consistency. 
Changes across module boundaries trigger domain events. We use the **Outbox Pattern** locally within each module's transaction boundary, ensuring no data loss or mismatches if the message broker goes down temporarily. The outbox processor seamlessly propagates events system-wide.
