using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class TranscriptIdException : DomainException
{
    public TranscriptIdException(Guid id)
        : base($"transcript_id: '{id}' is invalid.")
    {
    }
}
