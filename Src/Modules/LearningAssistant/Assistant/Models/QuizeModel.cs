using AI.Common.Core;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record QuizeModel : Entity<QuizeId>
{
    public LessonId LessonId { get; private set; } = default!;
    public QuizeStatus QuizeStatus { get; private set; } = default!;
    public double Score { get; private set; } = default!;
        // questons
    }
