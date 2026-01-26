using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class SummaryException : DomainException
{
    public SummaryException(string summary)
        : base($"summary: '{summary}' is invalid.")
    {
    }
}
