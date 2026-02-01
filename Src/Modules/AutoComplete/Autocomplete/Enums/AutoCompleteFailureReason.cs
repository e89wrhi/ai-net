namespace AutoComplete.Enums;

public enum AutoCompleteFailureReason
{
    ModelUnavailable = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
