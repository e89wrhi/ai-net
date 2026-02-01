namespace Payment.Enums;

public enum PaymentFailureReason
{
    InsufficientFunds = 0,
    CardDeclined = 1,
    Timeout = 2,
    ProviderError = 3,
}
