namespace AI.Common.Core;

/// <summary>
/// A wrapper that treats a domain event as an integration event.
/// Encapsulates a domain event to satisfy the IIntegrationEvent interface, allowing it to be published across boundaries.
/// Used by the EventDispatcher to automatically wrap domain events that are also intended to be integration events.
/// </summary>
public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : IIntegrationEvent
    where TDomainEventType : IDomainEvent;