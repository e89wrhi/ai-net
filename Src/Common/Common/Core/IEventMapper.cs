namespace AI.Common.Core;

/// <summary>
/// Interface for mapping domain events to other types of messages.
/// Defines methods to transform a domain event into an integration event or an internal command.
/// Implemented in specific modules to bridge domain changes with cross-module or external communication.
/// </summary>
public interface IEventMapper
{
    IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event);
    IInternalCommand? MapToInternalCommand(IDomainEvent @event);
}