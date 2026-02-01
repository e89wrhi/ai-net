using AI.Common.Core;
using AiOrchestration.ValueObjects;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Models;

public record LearningActivity : Entity<ActivityId>
{
    public LearningTopic Topic { get; private set; } = default!;
    public LearningContent Content { get; private set; } = default!;
    public LearningOutcome Outcome { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime ActivityAt { get; private set; }

    private LearningActivity() { }

    public static LearningActivity Create(
        ActivityId id,
        LearningTopic topic,
        LearningContent content,
        LearningOutcome outcome,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new LearningActivity
        {
            Id = id,
            Topic = topic,
            Content = content,
            Outcome = outcome,
            TokenUsed = tokenUsed,
            Cost = cost,
            ActivityAt = DateTime.UtcNow
        };
    }
}
