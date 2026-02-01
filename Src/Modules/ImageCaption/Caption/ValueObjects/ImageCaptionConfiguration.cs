using ImageCaption.Enums;

namespace ImageCaption.ValueObjects;

public record ImageCaptionConfiguration
{
    public CaptionDetailLevel DetailLevel { get; }
    public LanguageCode Language { get; }

    public ImageCaptionConfiguration(
        CaptionDetailLevel detailLevel,
        LanguageCode language)
    {
        DetailLevel = detailLevel;
        Language = language;
    }
}
