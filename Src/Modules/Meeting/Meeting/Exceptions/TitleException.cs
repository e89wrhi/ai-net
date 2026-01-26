using AI.Common.BaseExceptions;

namespace Meeting.Exceptions;

public class TitleException : DomainException
{
    public TitleException(string title)
        : base($"title: '{title}' is invalid.")
    {
    }
}
