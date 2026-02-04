using AI.Common.Core;
using Sentiment.Enums;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;

public record AnalyzeSentimentDetailedCommand(Guid UserId, string Text, string Language, SentimentDetailLevel DetailLevel, string? ModelId = null) : ICommand<AnalyzeSentimentDetailedCommandResult>;

public record AnalyzeSentimentDetailedCommandResult(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation, string ModelId, string? ProviderName);
