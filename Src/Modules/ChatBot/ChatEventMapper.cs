using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using ChatBot.Events;
using ChatBot.Features.DeleteChat.V1;
using ChatBot.Features.StartChat.V1;
using ChatBot.Models;

namespace ChatBot;


// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class ChatEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ChatSessionStartedDomainEvent e => new ChatSessionStarted(e.Id),
            MessageRecievedDomainEvent e => new MessageRecieved(e.Id),
            MessageRespondedDomainEvent e => new MessageRecieved(e.Id),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ChatSessionStartedDomainEvent e => new StartChatMongo(e.Id, e.MatchId, e.Title, e.Time,
            e.Type.ToString()),
            MessageRecievedDomainEvent e => new DeleteEventMongo(e.Id),
            _ => null
        };
    }
}