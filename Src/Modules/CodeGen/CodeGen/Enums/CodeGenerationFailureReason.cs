namespace CodeGen.Enums;

public enum CodeGenerationFailureReason
{
    UnsupportedLanguage = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
