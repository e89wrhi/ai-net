using AI.Common.Core;

namespace Translate;

public sealed class TranslateEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            // map to integration event here(if needed)
            // TranslationSessionStartedDomainEvent e => new TranslateSessionStarted(e.SessionId.Value),
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