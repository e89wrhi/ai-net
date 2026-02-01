using AI.Common.Core;
using Sentiment.ValueObjects;

namespace Sentiment.Events;

public record TextSentimentSessionCompletedDomainEvent(SentimentId Id): IDomainEvent;

