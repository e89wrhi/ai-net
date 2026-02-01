using AI.Common.BaseExceptions;

namespace Summary.Exceptions;

public class SummaryIdException : DomainException
{
    public SummaryIdException(Guid id)
        : base($"summary_id: '{id}' is invalid.")
    {
    }
}