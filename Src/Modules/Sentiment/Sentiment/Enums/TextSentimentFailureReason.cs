namespace Sentiment.Enums;

public enum TextSentimentFailureReason
{
    InvalidText = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
