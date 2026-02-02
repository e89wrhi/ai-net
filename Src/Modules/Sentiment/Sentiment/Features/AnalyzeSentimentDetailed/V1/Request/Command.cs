using AI.Common.Core;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;


public record AnalyzeSentimentDetailedCommand(string Text) : ICommand<AnalyzeSentimentDetailedCommandResult>;

public record AnalyzeSentimentDetailedCommandResult(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation);
