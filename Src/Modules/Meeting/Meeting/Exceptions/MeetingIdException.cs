using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class MeetingIdException : DomainException
{
    public MeetingIdException(Guid meeting_id)
        : base($"meeting_id: '{meeting_id}' is invalid.")
    {
    }
}
