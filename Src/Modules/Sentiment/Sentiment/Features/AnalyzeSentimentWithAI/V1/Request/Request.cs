namespace Sentiment.Features.AnalyzeSentimentWithAI.V1;

public record AnalyzeSentimentWithAIRequestDto(string Text);
public record AnalyzeSentimentWithAIResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score);
