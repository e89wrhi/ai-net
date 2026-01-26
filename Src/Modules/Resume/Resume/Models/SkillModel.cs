using AI.Common.Core;
using Resume.Enums;
using Resume.ValueObjects;

namespace Resume.Models;

public record SkillModel : Entity<SkillId>
{
    public ResumeId ResumeId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public SkillCategory Category { get; private set; } = default!;
    public ConfidenceScore ConfidenceScore { get; private set; } = default!;
}
