using AI.Common.Core;
using AiOrchestration.ValueObjects;
using AutoComplete.ValueObjects;

namespace AutoComplete.Models;

public record AutoCompleteRequest : Entity<AutoCompleteRequestId>
{
    public AutoCompletePrompt Prompt { get; private set; } = default!;
    public AutoCompleteSuggestion Suggestion { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime RequestedAt { get; private set; }

    private AutoCompleteRequest() { }

    public static AutoCompleteRequest Create(
        AutoCompleteRequestId id,
        AutoCompletePrompt prompt,
        AutoCompleteSuggestion suggestion,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new AutoCompleteRequest
        {
            Id = id,
            Prompt = prompt,
            Suggestion = suggestion,
            TokenUsed = tokenUsed,
            Cost = cost,
            RequestedAt = DateTime.UtcNow
        };
    }
}
