using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class LanguageCodeException : DomainException
{
    public LanguageCodeException(string lang)
        : base($"language: '{lang}' is invalid.")
    {
    }
}
