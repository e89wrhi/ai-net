namespace TextToSpeech.Dtos;

public class TextToSpeechDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string AiModelId { get; set; } = default!;
    public string SessionStatus { get; set; } = default!;
    public DateTime LastSentAt { get; set; }
    public int TotalTokens { get; set; }
}

