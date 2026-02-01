using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class CodeGenIdException : DomainException
{
    public CodeGenIdException(Guid id)
        : base($"codegen_id: '{id}' is invalid.")
    {
    }
}