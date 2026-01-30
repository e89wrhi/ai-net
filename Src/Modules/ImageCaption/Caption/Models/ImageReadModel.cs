namespace ImageCaption.Models;

public class ImageReadModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
    public long SizeInBytes { get; set; }
    public string Format { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
    public List<CaptionReadModel> Captions { get; set; } = new();
}

public class CaptionReadModel
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
    public string Language { get; set; } = default!;
    public double ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
}

