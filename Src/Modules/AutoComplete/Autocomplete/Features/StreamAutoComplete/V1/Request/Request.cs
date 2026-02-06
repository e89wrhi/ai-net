using AutoComplete.Enums;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public record StreamAutoCompleteRequestDto(string Prompt, CompletionMode Mode, string? ModelId = null);

