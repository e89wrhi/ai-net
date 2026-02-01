using Translate.Enums;
using Translate.Exceptions;

namespace Translate.ValueObjects;

public record TranslationConfiguration
{
    public LanguageCode SourceLanguage { get; }
    public LanguageCode TargetLanguage { get; }
    public TranslationDetailLevel DetailLevel { get; }

    public TranslationConfiguration(LanguageCode sourceLanguage, LanguageCode targetLanguage, TranslationDetailLevel detailLevel)
    {
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
        DetailLevel = detailLevel;
    }
}
