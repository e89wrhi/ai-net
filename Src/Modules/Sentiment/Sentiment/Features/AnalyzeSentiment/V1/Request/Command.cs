using AI.Common.Core;

namespace Sentiment.Features.AnalyzeSentiment.V1;

public record AnalyzeSentimentCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<AnalyzeSentimentCommandResult>;

public record AnalyzeSentimentCommandResult(Guid SessionId, Guid ResultId, string Sentiment, double Score, string ModelId, string? ProviderName);
