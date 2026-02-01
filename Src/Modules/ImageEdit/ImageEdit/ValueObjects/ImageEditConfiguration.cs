using ImageEdit.Enums;
using ImageEdit.Exceptions;

namespace ImageEdit.ValueObjects;

public record ImageEditConfiguration
{
    public ImageEditQuality Quality { get; }
    public ImageFormat OutputFormat { get; }

    public ImageEditConfiguration(
        ImageEditQuality quality,
        ImageFormat outputFormat)
    {
        Quality = quality;
        OutputFormat = outputFormat;
    }
}
