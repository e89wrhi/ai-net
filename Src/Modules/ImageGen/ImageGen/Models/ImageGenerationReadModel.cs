namespace ImageGen.Models;

public class ImageGenerationReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string AiModelId { get; set; } = default!;
    public string SessionStatus { get; set; } = default!;
    public DateTime LastSentAt { get; set; }
    public int TotalTokens { get; set; }
    public List<ImageGenReadModel> ImageGens { get; set; } = new();
}

public class ImageGenReadModel
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string Sender { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public int TokenUsed { get; set; }
}

