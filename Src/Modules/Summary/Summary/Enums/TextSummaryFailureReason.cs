namespace Summary.Enums;

public enum TextSummaryFailureReason
{
    InvalidText = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
