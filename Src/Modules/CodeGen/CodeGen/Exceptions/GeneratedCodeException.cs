using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class GeneratedCodeException : DomainException
{
    public GeneratedCodeException(string code)
        : base($"generated_code: '{code}' is invalid.")
    {
    }
}