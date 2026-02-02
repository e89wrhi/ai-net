namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;


public record AnalyzeSentimentDetailedRequestDto(string Text);
public record AnalyzeSentimentDetailedResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation);
