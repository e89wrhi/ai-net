using AI.Common.Core;
using Summary.Enums;

namespace Summary.Features.SummarizeText.V1;

public record SummarizeTextWithAICommand(string Text, SummaryDetailLevel DetailLevel, string Language) : ICommand<SummarizeTextWithAICommandResult>;

public record SummarizeTextWithAICommandResult(Guid SessionId, Guid ResultId, string Summary);
