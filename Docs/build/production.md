# 🚀 Production Deployment

Deploying the AI Platform to production requires careful planning for scaling, observability, and data persistence.

## Deployment Model

Although we build this as a **Modular Monolith**, the deployment can be scaled horizontally. The `.NET Aspire` setup is primarily for local development and CI/CD consistency. In production, we deploy via **Kubernetes**, **Azure Container Apps**, or **AWS ECS**.

## Infrastructure Requirements

To run the AI platform effectively in production, you will need the following backing services:

- **Compute**: Kubernetes Cluster (AKS/EKS/GKE) or Azure Container Apps.
- **Database**: Managed PostgreSQL (e.g., Azure Database for PostgreSQL, Amazon RDS).
- **Message Broker**: Managed RabbitMQ instances or Azure Service Bus.
- **Cache**: Redis Cluster for distributed caching, signalR backplane, and rate limiting.
- **Event Store**: EventStoreDB instance or cluster for robust domain event tracking.
- **Observability**: Application Insights, Datadog, or an OpenTelemetry collector pointing to Grafana/Prometheus.

## Best Practices

1. **Configuration & Secrets**: Use a secure vault (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) for injecting sensitive settings via environment variables. Do not store secrets in configuration files!
2. **Migrations**: Apply database migrations through an automated CI/CD pipeline using the EF Core CLI tools (or the `update-db.sh` script) before routing traffic to the newly deployed containers.
3. **Health Checks**: Always expose and monitor the `/health` and `/alive` probes. Use these endpoints to manage container lifecycle in Kubernetes.
4. **Resiliency**: Ensure Polly retry and circuit breaker policies are configured securely for all external outbound calls (especially to third-party LLM providers, which frequently throttle or limit connections).
5. **Rate Limiting**: Defend the public APIs with strict IP-based or tenant-based rate limiting to avoid billing abuse.
