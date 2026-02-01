using AI.Common.Core;
using Sentiment.Enums;
using Sentiment.ValueObjects;

namespace Sentiment.Events;

public record TextSentimentSessionFailedDomainEvent(SentimentId Id, TextSentimentFailureReason Reason): IDomainEvent;
