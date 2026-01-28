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

    private readonly List<QuizeModel> _quizes = new();
    public IReadOnlyCollection<QuizeModel> Quizes => _quizes.AsReadOnly();

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

        lesson.AddDomainEvent(new LearningAssistant.Events.LessonGeneratedDomainEvent(profileId, id, title, content));
        return lesson;
    }


    public void Complete()
    {
        IsCompleted = true;
        LastModified = DateTime.UtcNow;
    }

    public void AddQuize(QuizeModel quize)
    {
        _quizes.Add(quize);
    }
}
