using AI.Common.Core;
using Sentiment.ValueObjects;

namespace Sentiment.Events;

public record TextSentimentAnalyzedDomainEvent(SentimentId Id, SentimentResultId Result, string Sentiment): IDomainEvent;
