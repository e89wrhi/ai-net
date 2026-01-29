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

    private ProfileModel() { }

    public static ProfileModel Create(ProfileId id, string userId, string learningStyle, string level)
    {
        var profile = new ProfileModel
        {
            Id = id,
            UserId = userId,
            PreferredLearningStyle = learningStyle,
            CurrentLevel = level,
            LessonsCount = 0,
            ComplitionRate = 0,
            CreatedAt = DateTime.UtcNow
        };

        profile.AddDomainEvent(new LearningAssistant.Events.LearningProfileCreatedDomainEvent(id, userId));
        return profile;
    }

    public void AddLesson(LessonModel lesson)
    {
        _lessons.Add(lesson);
        LessonsCount = _lessons.Count;
        LastAccessedAt = DateTime.UtcNow;

        AddDomainEvent(new LearningAssistant.Events.LessonGeneratedDomainEvent(Id, lesson.Id, lesson.Title,
            lesson.Content));
    }

    public void AddQuizToLesson(LessonId lessonId, QuizModel quiz)
    {
        var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null) return;
        
        lesson.AddQuiz(quiz);
        AddDomainEvent(new LearningAssistant.Events.QuizGeneratedDomainEvent(lessonId, quiz.Id, quiz.Questions));
    }

    public void SubmitQuiz(LessonId lessonId, QuizId quizId, double score)
    {
        var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null) return;
        
        var quiz = lesson.Quizzes.FirstOrDefault(q => q.Id == quizId);
        if (quiz == null) return;
        
        quiz.Submit(score);
        AddDomainEvent(new LearningAssistant.Events.QuizCompletedDomainEvent(lessonId, quizId, score));
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        if (LessonsCount > 0)
        {
            var completedCount = _lessons.Count(l => l.IsCompleted);
            ComplitionRate = (double)completedCount / LessonsCount;
        }
        
        LastModified = DateTime.UtcNow;
        AddDomainEvent(new LearningAssistant.Events.ProgressUpdatedDomainEvent(Id, ComplitionRate));
    }
}
