using AI.Common.BaseExceptions;

namespace SpeechToText.Exceptions;

public class TranscriptException : DomainException
{
    public TranscriptException(string transcript)
        : base($"transcript: '{transcript}' is invalid.")
    {
    }
}