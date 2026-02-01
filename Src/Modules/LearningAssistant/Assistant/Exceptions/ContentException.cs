using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class ContentException : DomainException
{
    public ContentException(string content)
        : base($"content: '{content}' is invalid.")
    {
    }
}
