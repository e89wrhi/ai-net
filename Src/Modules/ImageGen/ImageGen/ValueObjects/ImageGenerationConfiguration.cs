using ImageGen.Enums;
using ImageGen.Exceptions;

namespace ImageGen.ValueObjects;


public record ImageGenerationConfiguration
{
    public ImageSize Size { get; }
    public ImageStyle Style { get; }
    public ImageQuality Quality { get; }
    public LanguageCode Language { get; }

    public ImageGenerationConfiguration(
        ImageSize size,
        ImageStyle style,
        ImageQuality quality,
        LanguageCode language)
    {
        Size = size;
        Style = style;
        Quality = quality;
        Language = language;
    }
}

