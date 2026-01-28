using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using ImageCaption.Features.DeleteChat.V1;
using ImageCaption.Features.StartChat.V1;
using ImageCaption.Models;

namespace ChatBot;


// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class ChatEventMapper : IEventMapper
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