using AI.Common.Contracts.EventBus.Sentiments;
using AI.Common.Core;
using Sentiment.Events;
using Sentiment.Features.DeleteSentiment.V1;
using Sentiment.Features.SendSentiment.V1;
using Sentiment.Features.StartSentiment.V1;

namespace Sentiment;

public sealed class SentimentEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            TextSentimentSessionStartedDomainEvent e => new SentimentSessionStarted(e.SessionId.Value),
            SentimentRecievedDomainEvent e => new SentimentRecieved(e.SentimentId.Value),
            SentimentRespondedDomainEvent e => new SentimentResponded(e.SentimentId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            TextSentimentSessionStartedDomainEvent e => new StartSentimentMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            SentimentRecievedDomainEvent e => new SendSentimentMongo(e.SessionId.Value, e.SentimentId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            SentimentRespondedDomainEvent e => new SendSentimentMongo(e.SessionId.Value, e.SentimentId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            SentimentSessionDeletedDomainEvent e => new GenerateSentimentMongo(e.Id.Value),
            _ => null
        };
    }
}