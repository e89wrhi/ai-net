using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record SentimentAnalyzed(Guid Id, string Text, string Sentiment, double Confidence) : IIntegrationEvent;
public record EmotionDetected(Guid Id, string Emotion) : IIntegrationEvent;
public record SentimentTrendDetected(Guid Id, string Trend) : IIntegrationEvent;
public record EntityExtracted(Guid Id, string EntityType, string Value) : IIntegrationEvent;
