using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using Resume.Features.DeleteChat.V1;
using Resume.Features.StartChat.V1;
using Resume.Models;

namespace Resume;


// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class ResumeEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            EventCreatedDomainEvent e => new EventCreated(e.Id),
            EventDeletedDomainEvent e => new EventDeleted(e.Id),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            EventCreatedDomainEvent e => new AddEventMongo(e.Id, e.MatchId, e.Title, e.Time,
            e.Type.ToString()),
            EventDeletedDomainEvent e => new DeleteEventMongo(e.Id),
            _ => null
        };
    }
}