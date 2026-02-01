namespace TextToSpeech.Enums;

public enum TextToSpeechFailureReason
{
    InvalidText = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
