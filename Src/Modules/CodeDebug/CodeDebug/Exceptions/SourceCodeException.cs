using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class SourceCodeException : DomainException
{
    public SourceCodeException(string code)
        : base($"source_code: '{code}' is invalid.")
    {
    }
}