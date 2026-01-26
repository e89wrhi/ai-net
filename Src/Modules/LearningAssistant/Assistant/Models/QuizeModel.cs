using AI.Common.Core;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record QuizeModel : Entity<QuizeId>
{
    public LessonId LessonId { get; private set; } = default!;
    public QuizeStatus QuizeStatus { get; private set; } = default!;
    public double Score { get; private set; } = default!;
    public string Questions { get; private set; } = default!;

    private QuizeModel() { }

    public static QuizeModel Create(QuizeId id, LessonId lessonId, string questions)
    {
        return new QuizeModel
        {
            Id = id,
            LessonId = lessonId,
            Questions = questions,
            QuizeStatus = QuizeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Submit(double score)
    {
        Score = score;
        QuizeStatus = QuizeStatus.Completed;
        LastModified = DateTime.UtcNow;
    }
}
