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

    private ImageModel() { }

    public static ImageModel Create(ImageId id, string userId, FileReference fileReference, int width, int height, long size, string format)
    {
        var image = new ImageModel
        {
            Id = id,
            UserId = userId,
            FileReference = fileReference,
            Width = width,
            Height = height,
            SizeInBytes = size,
            Format = format,
            ImageProcessingStatus = ImageProcessingStatus.Pending,
            UploadedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        image.AddDomainEvent(new ImageCaption.Events.ImageUploadedDomainEvent(id, userId, format));
        return image;
    }

    public void AddDetectedObject(DetectedObject obj)
    {
        _objects.Add(obj);
        ImageProcessingStatus = ImageProcessingStatus.Processing;
    }

    public void AddCaption(CaptionModel caption)
    {
        _captions.Add(caption);
        ProcessedAt = DateTime.UtcNow;
        ImageProcessingStatus = ImageProcessingStatus.Completed;

        AddDomainEvent(new ImageCaption.Events.CaptionGeneratedDomainEvent(Id, caption.Text));
        AddDomainEvent(new ImageCaption.Events.ImageAnalyzedDomainEvent(Id, _objects.Count));
    }
}
