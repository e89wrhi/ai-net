namespace ChatBot.Enums;

public enum ChatFailureReason
{
    ModelUnavailable = 0,
    TokenLimitExceeded = 1,
    Timeout = 2,
    ProviderError = 3,
}
