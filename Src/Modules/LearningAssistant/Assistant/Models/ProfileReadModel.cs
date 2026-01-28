namespace LearningAssistant.Models;

public class ProfileReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public List<LessonReadModel> Lessons { get; set; } = new();
}

public class LessonReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public List<QuizeReadModel> Quizes { get; set; } = new();
}

public class QuizeReadModel
{
    public Guid Id { get; set; }
    public string Question { get; set; } = default!;
    public string CorrectAnswer { get; set; } = default!;
    public string? UserAnswer { get; set; }
    public bool? IsCorrect { get; set; }
}

