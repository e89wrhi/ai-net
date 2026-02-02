namespace AutoComplete.Features.GenerateAICompletion.V1;

public record GenerateAutoCompleteRequestDto(string Prompt);
public record GenerateAutoCompleteResponseDto(string Completion, int TokensUsed, decimal EstimatedCost);
