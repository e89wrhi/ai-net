namespace LearningAssistant.Enums;

public enum LearningFailureReason
{
    ModelUnavailable = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
