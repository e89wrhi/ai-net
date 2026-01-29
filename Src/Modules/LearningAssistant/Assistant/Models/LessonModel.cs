using AI.Common.Core;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record LessonModel : Entity<LessonId>
{
    public ProfileId ProfileId { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public string Summary { get; private set; } = default!;
    public int EstimatedTimeMinutes { get; private set; } = default!;
    public bool IsCompleted { get; private set; } = default!;
    public DifficultyLevel DifficultyLevel { get; private set; } = default!;

    private readonly List<QuizModel> _quizzes = new();
    public IReadOnlyCollection<QuizModel> Quizzes => _quizzes.AsReadOnly();

    private LessonModel() { }

    public static LessonModel Create(LessonId id, ProfileId profileId, string title, string content, DifficultyLevel level)
    {
        var lesson = new LessonModel
        {
            Id = id,
            ProfileId = profileId,
            Title = title,
            Content = content,
            DifficultyLevel = level,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        return lesson;
    }


    public void Complete()
    {
        IsCompleted = true;
        LastModified = DateTime.UtcNow;
    }

    public void AddQuiz(QuizModel quiz)
    {
        _quizzes.Add(quiz);
    }
}
