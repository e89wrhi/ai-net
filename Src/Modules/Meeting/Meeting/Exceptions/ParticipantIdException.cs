using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class ParticipantIdException : DomainException
{
    public ParticipantIdException(Guid participant_id)
        : base($"participant_id: '{participant_id}' is invalid.")
    {
    }
}
