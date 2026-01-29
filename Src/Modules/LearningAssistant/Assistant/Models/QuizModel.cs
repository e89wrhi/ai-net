using AI.Common.Core;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record QuizModel : Entity<QuizId>
{
    public LessonId LessonId { get; private set; } = default!;
    public QuizStatus QuizStatus { get; private set; } = default!;
    public double Score { get; private set; } = default!;
    public string Questions { get; private set; } = default!;

    private QuizModel() { }

    public static QuizModel Create(QuizId id, LessonId lessonId, string questions)
    {
        var quiz = new QuizModel
        {
            Id = id,
            LessonId = lessonId,
            Questions = questions,
            QuizStatus = QuizStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        return quiz;
    }

    public void Submit(double score)
    {
        Score = score;
        QuizStatus = QuizStatus.Completed;
        LastModified = DateTime.UtcNow;
    }

}
