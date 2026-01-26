using AI.Common.Core;
using ImageCaption.Enums;
using ImageCaption.ValueObjects;

namespace ImageCaption.Models;

public record ImageModel : Aggregate<ImageId>
{
    public FileReference FileReference { get; private set; } = default!;

    public ImageProcessingStatus ImageProcessingStatus { get; private set; } = default!;

    public string UserId { get; private set; } = default!;
    public int Width { get; private set; } = default!;
    public int Height { get; private set; } = default!;
    public long SizeInBytes { get; private set; } = default!;
    public string Format { get; private set; } = default!;
    public DateTime UploadedAt { get; private set; } = default!;

    public DateTime ProcessedAt { get; private set; } = default!;


    private readonly List<DetectedObject> _objects = new();
    public IReadOnlyCollection<DetectedObject> Objects => _objects.AsReadOnly();


    private readonly List<CaptionModel> _captions = new();
    public IReadOnlyCollection<CaptionModel> Captions => _captions.AsReadOnly();
}
