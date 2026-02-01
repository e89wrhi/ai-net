using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class PaymentIdException : DomainException
{
    public PaymentIdException(Guid id)
        : base($"payment_id: '{id}' is invalid.")
    {
    }
}
