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
    public static SkillModel Create(SkillId id, ResumeId resumeId, string name, SkillCategory category, ConfidenceScore confidence)
    {
        return new SkillModel
        {
            Id = id,
            ResumeId = resumeId,
            Name = name,
            Category = category,
            ConfidenceScore = confidence
        };
    }
}

