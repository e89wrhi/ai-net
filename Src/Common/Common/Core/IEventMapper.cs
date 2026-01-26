namespace AI.Common.Core;

public interface IEventMapper
{
    IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event);
    IInternalCommand? MapToInternalCommand(IDomainEvent @event);
}