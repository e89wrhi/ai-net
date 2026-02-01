using AI.Common.BaseExceptions;

namespace TextToSpeech.Exceptions;

public class LanguageCodeException : DomainException
{
    public LanguageCodeException(string lang)
        : base($"language: '{lang}' is invalid.")
    {
    }
}
