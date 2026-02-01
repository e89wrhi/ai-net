using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class CodeGenPromptException : DomainException
{
    public CodeGenPromptException(string prompt)
        : base($"prompt: '{prompt}' is invalid.")
    {
    }
}