using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class IssueCountException : DomainException
{
    public IssueCountException(int count)
        : base($"issue_count: '{count}' is invalid.")
    {
    }
}
