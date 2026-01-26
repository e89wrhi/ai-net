using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class ParticipantNotFoundException : DomainException
{
    public ParticipantNotFoundException(Guid participantId)
        : base($"participant: '{participantId}' not found.")
    {
    }
}

public class ParticipantAlreadyExistException : DomainException
{
    public ParticipantAlreadyExistException(Guid participantId)
        : base($"participant: '{participantId}' already exist.")
    {
    }
}
