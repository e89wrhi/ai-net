using MassTransit;

namespace AI.Common.Core;

/// <summary>
/// An event meant for other microservices or modules to hear about.
/// These are published to a message bus (like RabbitMQ or Azure Service Bus).
/// </summary>
[ExcludeFromTopology]
public interface IIntegrationEvent : IEvent
{
}