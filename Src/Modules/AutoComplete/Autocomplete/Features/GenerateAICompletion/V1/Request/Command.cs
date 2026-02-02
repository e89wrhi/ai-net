using AI.Common.Core;

namespace AutoComplete.Features.GenerateAICompletion.V1;


public record GenerateAICompletionCommand(Guid UserId, string Prompt) : ICommand<GenerateAICompletionCommandResult>;

public record GenerateAICompletionCommandResult(string Completion, int TokensUsed, decimal EstimatedCost);
