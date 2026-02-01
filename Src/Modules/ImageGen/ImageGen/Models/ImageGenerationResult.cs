using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageGen.Enums;
using ImageGen.ValueObjects;

namespace ImageGen.Models;

public record ImageGenerationResult : Entity<ImageGenResultId>
{
    public ImageGenerationPrompt Prompt { get; private set; } = default!;
    public GeneratedImage Image { get; private set; } = default!;
    public ImageSize Size { get; private set; }
    public ImageStyle Style { get; private set; }
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime GeneratedAt { get; private set; }

    private ImageGenerationResult() { }

    public static ImageGenerationResult Create(
        ImageGenResultId id,
        ImageGenerationPrompt prompt,
        GeneratedImage image,
        ImageSize size,
        ImageStyle style,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new ImageGenerationResult
        {
            Id = id,
            Prompt = prompt,
            Image = image,
            Size = size,
            Style = style,
            TokenUsed = tokenUsed,
            Cost = cost,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
