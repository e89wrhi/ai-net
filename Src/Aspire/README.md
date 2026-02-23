# .NET Aspire Orchestration Layer

## 🚀 Overview
This folder contains the **.NET Aspire** implementation for the application. It acts as the "glue" that binds our microservices, databases, messaging systems, and observability tools together into a unified development and deployment environment.

Instead of manually starting multiple Docker containers or projects, .NET Aspire allows us to define and run the entire system with a single command.

---

## 🏗️ Folder Structure

### 🏠 [AppHost](./src/AppHost)
The **AppHost** is the orchestrator. It uses the .NET Distributed Application model to define:
- **Databases**: PostgreSQL (with flight, passenger, identity, and persist-message DBs), and Redis.
- **Event Sourcing**: EventStoreDB configuration.
- **Messaging**: RabbitMQ with its management interface.
- **Observability Stack**: 
  - **Jaeger & Zipkin**: For distributed tracing.
  - **Prometheus**: For metrics collection.
  - **Grafana**: For visualization (pre-configured with dashboards).
  - **ELK Stack (Elasticsearch/Kibana)**: For logging.
  - **Loki, Tempo, & OpenTelemetry Collector**: For advanced cloud-native monitoring.
- **Applications**: The main `Api` project and its dependencies.

### 🛠️ [ServiceDefaults](./src/ServiceDefaults)
The **ServiceDefaults** project provides a shared set of extensions that ensure all our services behave consistently. This includes:
- **Health Checks**: Standardized monitoring logic.
- **Observability**: Consistent OpenTelemetry setup for logs, traces, and metrics.
- **Resiliency**: Default HttpClient policies (retries, circuit breakers).
- **Service Discovery**: Automatic resolution of service endpoints.

---

## ⚡ Getting Started
1. Ensure you have the **.NET Aspire workload** installed.
2. Set the `AppHost` project as your startup project.
3. Run the project (`dotnet run` or F5 in IDE).
4. Browse the **Aspire Dashboard** to see the status of all containers, logs, and distributed traces in real-time.

---

## 📊 Observability Features
- **Centralized Logs**: View logs from all modules in one place.
- **Trace Visualization**: Follow a request as it travels from the API through the event bus to various handlers.
- **Metrics Dashboards**: Pre-configured Grafana dashboards for monitoring system performance and health.
