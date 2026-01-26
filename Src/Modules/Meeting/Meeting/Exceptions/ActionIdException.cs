using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class ActionIdException : DomainException
{
    public ActionIdException(Guid action_id)
        : base($"action_id: '{action_id}' is invalid.")
    {
    }
}
