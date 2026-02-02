using AI.Common.Core;

namespace Sentiment.Features.AnalyzeSentiment.V1;

public record AnalyzeSentimentWithAICommand(string Text) : ICommand<AnalyzeSentimentWithAICommandResult>;

public record AnalyzeSentimentWithAICommandResult(Guid SessionId, Guid ResultId, string Sentiment, double Score);
