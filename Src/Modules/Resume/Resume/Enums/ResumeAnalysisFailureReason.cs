namespace Resume.Enums;

public enum ResumeAnalysisFailureReason
{
    InvalidResume = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
