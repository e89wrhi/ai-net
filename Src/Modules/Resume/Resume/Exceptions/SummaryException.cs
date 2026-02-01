using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SummaryException : DomainException
{
    public SummaryException(string summary)
        : base($"summary: '{summary}' is invalid.")
    {
    }
}
