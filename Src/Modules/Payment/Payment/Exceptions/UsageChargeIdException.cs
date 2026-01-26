using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class UsageChargeIdException : DomainException
{
    public UsageChargeIdException(Guid usage_charge_id)
        : base($"usage_charge_id: '{usage_charge_id}' is invalid.")
    {
    }
}
