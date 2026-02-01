using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class LanguageCodeException : DomainException
{
    public LanguageCodeException(string lang)
        : base($"language: '{lang}' is invalid.")
    {
    }
}
