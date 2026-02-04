using AutoComplete.Enums;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record GenerateAutoCompleteRequestDto(string Prompt, CompletionMode Mode, string? ModelId = null);
public record GenerateAutoCompleteResponseDto(string Completion, long TokensUsed, decimal EstimatedCost, string ModelId, string? ProviderName);
