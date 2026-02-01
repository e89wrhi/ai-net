using AI.Common.BaseExceptions;

namespace TextToSpeech.Exceptions;

public class SythesizedSpeechException : DomainException
{
    public SythesizedSpeechException(string speech)
        : base($"sythesized_speech: '{speech}' is invalid.")
    {
    }
}