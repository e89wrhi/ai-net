namespace Translate.Models;

public class TranslationSessionReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string AiModelId { get; set; } = default!;
    public string SessionStatus { get; set; } = default!;
    public DateTime LastSentAt { get; set; }
    public int TotalTokens { get; set; }
    public List<TranslateReadModel> Translates { get; set; } = new();
}

public class TranslateReadModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string Sender { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public int TokenUsed { get; set; }
}

