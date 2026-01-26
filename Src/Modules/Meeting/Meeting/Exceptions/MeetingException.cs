using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class MeetingNotFoundException : DomainException
{
    public MeetingNotFoundException(Guid meetingId)
        : base($"meeting: '{meetingId}' not found.")
    {
    }
}

public class MeetingAlreadyExistException : DomainException
{
    public MeetingAlreadyExistException(Guid meetingId)
        : base($"meeting: '{meetingId}' already exist.")
    {
    }
}
