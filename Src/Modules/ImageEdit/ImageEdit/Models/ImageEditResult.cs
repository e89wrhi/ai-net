using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageEdit.Enums;
using ImageEdit.ValueObjects;

namespace ImageEdit.Models;

public record ImageEditResult : Entity<ImageEditResultId>
{
    public ImageSource OriginalImage { get; private set; } = default!;
    public EditedImage ResultImage { get; private set; } = default!;
    public ImageEditPrompt Prompt { get; private set; } = default!;
    public EditOperation Operation { get; private set; }
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime EditedAt { get; private set; }

    private ImageEditResult() { }

    public static ImageEditResult Create(
        ImageEditResultId id,
        ImageSource originalImage,
        EditedImage resultImage,
        ImageEditPrompt prompt,
        EditOperation operation,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new ImageEditResult
        {
            Id = id,
            OriginalImage = originalImage,
            ResultImage = resultImage,
            Prompt = prompt,
            Operation = operation,
            TokenUsed = tokenUsed,
            Cost = cost,
            EditedAt = DateTime.UtcNow
        };
    }
}
