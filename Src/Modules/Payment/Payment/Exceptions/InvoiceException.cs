using AI.Common.BaseExceptions;

namespace Payment.Exceptions;

public class InvoiceException : DomainException
{
    public InvoiceException(string invoice)
        : base($"invoice: '{invoice}' is invalid.")
    {
    }
}
