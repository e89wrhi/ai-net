using AI.Common.Core;
using Resume.Enums;
using Resume.ValueObjects;

namespace Resume.Models;

public record SuggestionModel : Entity<SuggestionId>
{
    public ResumeId ResumeId { get; private set; } = default!;
    public SuggestionType Type { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public int Priority { get; private set; } = default!;
    public bool IsImplemented { get; private set; } = default!;
}
