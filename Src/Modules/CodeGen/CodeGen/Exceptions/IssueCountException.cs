using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class IssueCountException : DomainException
{
    public IssueCountException(int count)
        : base($"issue_count: '{count}' is invalid.")
    {
    }
}
