using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class CodeDebugNotFoundException : DomainException
{
    public CodeDebugNotFoundException(Guid codedebugId)
        : base($"codedebug: '{codedebugId}' not found.")
    {
    }
}

public class CodeDebugAlreadyExistException : DomainException
{
    public CodeDebugAlreadyExistException(Guid codedebugId)
        : base($"codedebug: '{codedebugId}' already exist.")
    {
    }
}
