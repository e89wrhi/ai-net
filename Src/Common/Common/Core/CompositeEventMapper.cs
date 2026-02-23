namespace AI.Common.Core;

/// <summary>
/// A composite implementation of IEventMapper that aggregates multiple event mappers.
/// It iterates through a collection of mappers to find the first one that can map a domain event to an integration event or internal command.
/// Used in the event dispatching process to provide a unified way to map events across different modules.
/// </summary>
public class CompositeEventMapper : IEventMapper
{
    private readonly IEnumerable<IEventMapper> _mappers;

    public CompositeEventMapper(IEnumerable<IEventMapper> mappers)
    {
        _mappers = mappers;
    }

    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        foreach (var mapper in _mappers)
        {
            var integrationEvent = mapper.MapToIntegrationEvent(@event);
            if (integrationEvent is not null)
                return integrationEvent;
        }

        return null;
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        foreach (var mapper in _mappers)
        {
            var internalCommand = mapper.MapToInternalCommand(@event);
            if (internalCommand is not null)
                return internalCommand;
        }

        return null;
    }
}