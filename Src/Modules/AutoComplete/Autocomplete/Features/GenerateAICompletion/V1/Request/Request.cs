namespace AutoComplete.Features.GenerateAICompletion.V1;


public record GenerateAICompletionRequestDto(string Prompt);
public record GenerateAICompletionResponseDto(string Completion, int TokensUsed, decimal EstimatedCost);
