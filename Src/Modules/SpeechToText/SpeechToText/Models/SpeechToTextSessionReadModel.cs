namespace SpeechToText.Models;

public class SpeechToTextSessionReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string AiModelId { get; set; } = default!;
    public string SessionStatus { get; set; } = default!;
    public DateTime LastSentAt { get; set; }
    public int TotalTokens { get; set; }
    public List<SpeechToTextReadModel> SpeechToTexts { get; set; } = new();
}

public class SpeechToTextReadModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string Sender { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public int TokenUsed { get; set; }
}

