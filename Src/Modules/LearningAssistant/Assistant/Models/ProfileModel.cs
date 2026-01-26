using AI.Common.Core;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record ProfileModel : Aggregate<ProfileId>
{

    private readonly List<LessonModel> _lessons = new();
    public IReadOnlyCollection<LessonModel> Lessons => _lessons.AsReadOnly();

    public int LessonsCount { get; private set; } = default!;

    public double ComplitionRate { get; private set; } = default!;

    public DateTime? LastAccessedAt { get; private set; } = default!;
}
