using AI.Common.Core;
using ImageCaption.ValueObjects;


namespace ImageCaption.Models;

public record CaptionModel : Entity<CaptionId>
{
    public ImageId ImageId { get; private set; } = default!;
    public string Text { get; private set; } = default!;
    public double ConfidenceScore { get; private set; } = default!;
    public string Language { get; private set; } = default!;
}
