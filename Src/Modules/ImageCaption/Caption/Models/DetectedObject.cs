using AI.Common.Core;
using ImageCaption.ValueObjects;

namespace ImageCaption.Models;

public record DetectedObject : Entity<DetectedObjectId>
{
    public ImageId ImageId { get; private set; } = default!;
    public string Label { get; private set; } = default!;
    public string BoundingBox { get; private set; } = default!;
    public string Probability { get; private set; } = default!;

}
