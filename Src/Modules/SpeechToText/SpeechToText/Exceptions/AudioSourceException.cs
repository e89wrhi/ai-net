using AI.Common.BaseExceptions;

namespace SpeechToText.Exceptions;

public class AudioSourceException : DomainException
{
    public AudioSourceException(string source)
        : base($"audio_source: '{source}' is invalid.")
    {
    }
}
