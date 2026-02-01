using AI.Common.BaseExceptions;

namespace Summary.Exceptions;

public class SummaryNotFoundException : DomainException
{
    public SummaryNotFoundException(Guid summaryId)
        : base($"summary: '{summaryId}' not found.")
    {
    }
}

public class SummaryAlreadyExistException : DomainException
{
    public SummaryAlreadyExistException(Guid summaryId)
        : base($"summary: '{summaryId}' already exist.")
    {
    }
}
