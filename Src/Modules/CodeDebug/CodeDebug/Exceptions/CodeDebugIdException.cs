using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class CodeDebugIdException : DomainException
{
    public CodeDebugIdException(Guid id)
        : base($"codedebug_id: '{id}' is invalid.")
    {
    }
}