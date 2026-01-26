using AI.Common.Core;
using LearningAssistant.ValueObjects;


namespace LearningAssistant.Models;

public record ProfileModel : Aggregate<ProfileId>
{

    private readonly List<LessonModel> _lessons = new();
    public IReadOnlyCollection<LessonModel> Lessons => _lessons.AsReadOnly();

    public string UserId { get; private set; } = default!;
    public string PreferredLearningStyle { get; private set; } = default!;
    public string CurrentLevel { get; private set; } = default!;
    public int LessonsCount { get; private set; } = default!;

    public double ComplitionRate { get; private set; } = default!;

    public DateTime? LastAccessedAt { get; private set; } = default!;
}
