using SpeechToText.Enums;
using SpeechToText.Exceptions;

namespace SpeechToText.ValueObjects;

public record SpeechToTextConfiguration
{
    public LanguageCode Language { get; }
    public bool IncludePunctuation { get; }
    public SpeechToTextDetailLevel DetailLevel { get; }

    public SpeechToTextConfiguration(LanguageCode language, bool includePunctuation, SpeechToTextDetailLevel detailLevel)
    {
        Language = language;
        IncludePunctuation = includePunctuation;
        DetailLevel = detailLevel;
    }
}
