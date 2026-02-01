using AiOrchestration.ValueObjects;
using AutoComplete.Enums;
using AutoComplete.Exceptions;

namespace AutoComplete.ValueObjects;

public record AutoCompleteConfiguration
{
    public Temperature Temperature { get; }
    public TokenCount MaxTokens { get; }
    public CompletionMode Mode { get; }

    public AutoCompleteConfiguration(
        Temperature temperature,
        TokenCount maxTokens,
        CompletionMode mode)
    {
        Temperature = temperature;
        MaxTokens = maxTokens;
        Mode = mode;
    }
}
