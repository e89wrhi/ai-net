using AI.Common.Core;
using AutoComplete.Enums;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record GenerateAutoCompleteCommand(Guid UserId, string Prompt, CompletionMode Mode, string? ModelId = null) : ICommand<GenerateAutoCompleteCommandResult>;

public record GenerateAutoCompleteCommandResult(string Completion, long TokensUsed, decimal EstimatedCost, string ModelId, string? ProviderName);

