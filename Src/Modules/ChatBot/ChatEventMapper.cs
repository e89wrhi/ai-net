using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using ChatBot.Events;
using ChatBot.Features.DeleteChat.V1;
using ChatBot.Features.SendMessage.V1;
using ChatBot.Features.StartChat.V1;

namespace ChatBot;

public sealed class ChatEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ChatSessionStartedDomainEvent e => new ChatSessionStarted(e.SessionId.Value),
            MessageRecievedDomainEvent e => new MessageRecieved(e.MessageId.Value),
            MessageRespondedDomainEvent e => new MessageResponded(e.MessageId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ChatSessionStartedDomainEvent e => new StartChatMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            MessageRecievedDomainEvent e => new SendMessageMongo(e.SessionId.Value, e.MessageId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            MessageRespondedDomainEvent e => new SendMessageMongo(e.SessionId.Value, e.MessageId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            ChatSessionDeletedDomainEvent e => new DeleteChatMongo(e.Id.Value),
            _ => null
        };
    }
}