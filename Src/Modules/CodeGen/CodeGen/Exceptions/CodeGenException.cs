using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class CodeGenNotFoundException : DomainException
{
    public CodeGenNotFoundException(Guid codegenId)
        : base($"codegen: '{codegenId}' not found.")
    {
    }
}

public class CodeGenAlreadyExistException : DomainException
{
    public CodeGenAlreadyExistException(Guid codegenId)
        : base($"codegen: '{codegenId}' already exist.")
    {
    }
}
