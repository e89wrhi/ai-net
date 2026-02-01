using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class CodeDebugSummaryException : DomainException
{
    public CodeDebugSummaryException(string summary)
        : base($"codedebug_summary: '{summary}' is invalid.")
    {
    }
}