using AI.Common.Core;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record GenerateAutoCompleteCommand(Guid UserId, string Prompt) : ICommand<GenerateAutoCompleteCommandResult>;

public record GenerateAutoCompleteCommandResult(string Completion, long TokensUsed, decimal EstimatedCost, string ModelId, string? ProviderName);

