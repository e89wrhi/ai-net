namespace SpeechToText.Enums;

public enum SpeechToTextFailureReason
{
    InvalidAudio = 0,
    UnsupportedFormat = 1,
    TokenLimitExceeded = 2,
    Timeout = 3,
    ProviderError = 4,
}
