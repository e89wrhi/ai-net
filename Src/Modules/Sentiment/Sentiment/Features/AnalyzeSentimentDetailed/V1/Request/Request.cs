using Sentiment.Enums;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;

public record AnalyzeSentimentDetailedRequestDto(string Text, string Language, SentimentDetailLevel DetailLevel, string? ModelId = null);
public record AnalyzeSentimentDetailedResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation, string ModelId, string? ProviderName);
