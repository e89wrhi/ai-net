using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class ProgrammingLanguageException : DomainException
{
    public ProgrammingLanguageException(string lang)
        : base($"programming_language: '{lang}' is invalid.")
    {
    }
}