using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class TranscriptException : DomainException
{
    public TranscriptException(string transcript)
        : base($"transcript: '{transcript}' is invalid.")
    {
    }
}
