using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class UsageChargeException : DomainException
{
    public UsageChargeException(string usage_charge)
        : base($"usage_charge: '{usage_charge}' is invalid.")
    {
    }
}
