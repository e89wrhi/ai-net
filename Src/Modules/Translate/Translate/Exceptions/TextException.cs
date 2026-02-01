using AI.Common.BaseExceptions;

namespace Translate.Exceptions;

public class TextException : DomainException
{
    public TextException(string txt)
        : base($"text: '{txt}' is invalid.")
    {
    }
}