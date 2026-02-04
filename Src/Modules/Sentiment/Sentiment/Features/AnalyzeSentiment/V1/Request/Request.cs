namespace Sentiment.Features.AnalyzeSentiment.V1;

public record AnalyzeSentimentRequestDto(string Text, string? ModelId = null);
public record AnalyzeSentimentResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score, string ModelId, string? ProviderName);
