using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageCaption.ValueObjects;

namespace ImageCaption.Models;

public record ImageCaptionResult : Entity<ImageCaptionResultId>
{
    public ImageSource Image { get; private set; } = default!;
    public CaptionText Caption { get; private set; } = default!;
    public CaptionConfidence Confidence { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime CaptionedAt { get; private set; }

    private ImageCaptionResult() { }

    public static ImageCaptionResult Create(
        ImageCaptionResultId id,
        ImageSource image,
        CaptionText caption,
        CaptionConfidence confidence,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new ImageCaptionResult
        {
            Id = id,
            Image = image,
            Caption = caption,
            Confidence = confidence,
            TokenUsed = tokenUsed,
            Cost = cost,
            CaptionedAt = DateTime.UtcNow
        };
    }
}
