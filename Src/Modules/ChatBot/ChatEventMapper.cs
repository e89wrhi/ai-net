using AI.Common.Core;

namespace ChatBot;

public sealed class ChatEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            // map to integration event here(if needed)
            // DomainEvent e => new (e.Id.Value),
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