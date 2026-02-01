using AI.Common.BaseExceptions;

namespace Summary.Exceptions;

public class TextException : DomainException
{
    public TextException(string txt)
        : base($"text: '{txt}' is invalid.")
    {
    }
}