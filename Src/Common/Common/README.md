# Building Blocks Layer (Common Infrastructure)

## 📋 Overview
The `Common` directory serves as the **shared infrastructure layer** for the application. It contains reusable components, base classes, and abstractions that enforce architectural consistency and handle cross-cutting concerns across all functional modules.

By centralizing these "Building Block," we ensure that every module follows the same patterns for data persistence, messaging, observability, and error handling.

---

## 🏗️ Core Architectural Modules

### 🧱 [Core](./Core)
The heart of our DDD and CQRS implementation.
- **DDD Patterns**: Base classes for `AggregateRoot`, `Entity`, `ValueObject`, and `DomainEvent`.
- **CQRS**: Interfaces for `ICommand`, `IQuery`, and their respective handlers using MediatR.
- **Pagination**: Standardized models (`PageList`) and query extensions for handling paged data.

### ✉️ [Messaging & Events](./MassTransit)
Infrastructure for reliable asynchronous communication.
- **MassTransit**: Configures the message bus (RabbitMQ) with consistent retry and error-handling policies.
- **[Contracts](./Contracts)**: Shared integration event definitions that allow modules to communicate without being tightly coupled.
- **[PersistMessageProcessor](./PersistMessageProcessor)**: Implementation of the **Outbox** and **Inbox** patterns to guarantee eventual consistency and message idempotency.

### 💾 [Persistence](./EFCore)
Abstracted data access layers for various storage technologies.
- **[EFCore](./EFCore)**: Base `DbContext` and repository patterns for relational databases (PostgreSQL), including support for transactional behaviors.
- **[EventStoreDB](./EventStoreDB)**: Specialized components for **Event Sourcing**, including stream name mapping, snapshots, and projection publishers.

---

## 🌐 Web & API Infrastructure

### 🕸️ [Web](./Web)
Standardizes how our APIs are exposed.
- **Versioning**: Integrated API versioning (v1, v2, etc.) for both Minimal APIs and MVC.
- **Routing**: Slugified URL parameters for cleaner, SEO-friendly endpoints.
- **Base Controllers**: Pre-configured controllers with access to common services like `IMediator` and `IMapper`.

### 🛡️ [Security & Validation](./Validation)
- **[Jwt](./Jwt)**: Extensions for handling JWT-based authentication.
- **[Validation](./Validation)**: Automated pipeline behavior that uses **FluentValidation** to ensure all incoming requests are valid before hitting a handler.
- **[ProblemDetails](./ProblemDetails)**: RFC 7807 compliant error handling that returns structured, machine-readable error responses.

---

## 📊 Observability & Resiliency

### 🔍 [OpenTelemetry & Logging](./OpenTelemetryCollector)
- **Tracing & Metrics**: Integrated OpenTelemetry instrumentation to track requests across module boundaries.
- **[Logging](./Logging)**: Structured logging behavior that automatically records request/response metadata.

### 🛡️ [Resiliency & Utils](./Polly)
- **[Polly](./Polly)**: Extension methods for applying retry and circuit-breaker policies.
- **[Caching](./Caching)**: MediatR behaviors for transparent response caching (Distributed or In-Memory).
- **[HealthCheck](./HealthCheck)**: Automated health monitoring for dependencies like databases and message brokers.

---

## 🧪 [TestBase](./TestBase)
The foundation for our testing strategy.
- **TestContainers**: Automatically spins up real Docker containers (PostgreSQL, RabbitMQ) during integration tests to ensure tests run against realistic environments.
- **Test Fixtures**: Base classes that simplify the setup of `WebApplicationFactory` and dependency injection for unit and integration tests.

---

## 🚀 How to Use
1. **Inherit**: Use the base classes in `Core` or `EFCore` when building new domain entities or repositories.
2. **Inject**: Register services using the provided `IServiceCollection` extensions found in each module's `Extensions.cs` file.
3. **Configure**: Set up options in your `appsettings.json` using the strongly-typed options classes (e.g., `PostgresOptions`).
