namespace ImageGen.Enums;

public enum ImageGenerationFailureReason
{
    UnsafePrompt = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}

