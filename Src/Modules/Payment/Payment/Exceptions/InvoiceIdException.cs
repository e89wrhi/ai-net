using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class InvoiceIdException : DomainException
{
    public InvoiceIdException(Guid invoice_id)
        : base($"invoice_id: '{invoice_id}' is invalid.")
    {
    }
}
