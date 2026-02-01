namespace Meeting.Enums;

public enum MeetingAnalysisFailureReason
{
    ModelUnavailable = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
