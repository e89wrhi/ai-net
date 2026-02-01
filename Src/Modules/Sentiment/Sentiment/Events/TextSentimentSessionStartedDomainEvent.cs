using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Sentiment.ValueObjects;

namespace Sentiment.Events;

public record TextSentimentSessionStartedDomainEvent(SentimentId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
