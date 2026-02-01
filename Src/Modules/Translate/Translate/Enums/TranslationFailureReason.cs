namespace Translate.Enums;

public enum TranslationFailureReason
{
    InvalidText = 0,
    UnsupportedLanguage = 1,
    TokenLimitExceeded = 2,
    Timeout = 3,
    ProviderError = 4,
}
