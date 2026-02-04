namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record GenerateAutoCompleteRequestDto(string Prompt, string? ModelId = null);
public record GenerateAutoCompleteResponseDto(string Completion, long TokensUsed, decimal EstimatedCost);
