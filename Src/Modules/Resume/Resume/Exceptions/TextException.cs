using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class TextException : DomainException
{
    public TextException(string text)
        : base($"text: '{text}' is invalid.")
    {
    }
}
