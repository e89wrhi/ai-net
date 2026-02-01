using AI.Common.BaseExceptions;

namespace Sentiment.Exceptions;

public class TextException : DomainException
{
    public TextException(string txt)
        : base($"text: '{txt}' is invalid.")
    {
    }
}
