using AI.Common.Core;
using Summary.Enums;

namespace Summary.Features.SummarizeText.V1;

public record SummarizeTextCommand(Guid UserId, string Text, SummaryDetailLevel DetailLevel, string Language, string? ModelId = null) : ICommand<SummarizeTextCommandResult>;

public record SummarizeTextCommandResult(Guid SessionId, Guid ResultId, string Summary, string ModelId, string? ProviderName);
