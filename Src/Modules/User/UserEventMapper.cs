using AI.Common.Core;


namespace User;


// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class UserEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            // map to integration event here(if needed)
            // UserActivityTrackedDomainEvent e => new AI.Contracts.EventBus.Messages.UserActivityAdded(e.ActivityId.Value, e.UserId.Value, e.Module.ToString(), e.Action),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            // map domain events to internal commands to handle changes
            // DomainEvent e => new MethodName(e.SessionId.Value),
            _ => null
        };
    }
}

