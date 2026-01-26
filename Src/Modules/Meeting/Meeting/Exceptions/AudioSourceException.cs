using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class AudioSourceException : DomainException
{
    public AudioSourceException(string audio_source)
        : base($"audio_source: '{audio_source}' is invalid.")
    {
    }
}
