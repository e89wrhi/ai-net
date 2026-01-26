using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class ActionException : DomainException
{
    public ActionException(string action_)
        : base($"action_: '{action_}' is invalid.")
    {
    }
}
