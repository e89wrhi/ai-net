using AI.Common.Core;

namespace AutoComplete.Features.GenerateAICompletion.V1;

public record GenerateAutoCompleteCommand(Guid UserId, string Prompt) : ICommand<GenerateAutoCompleteCommandResult>;

public record GenerateAutoCompleteCommandResult(string Completion, int TokensUsed, decimal EstimatedCost);
