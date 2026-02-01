using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class TopicException : DomainException
{
    public TopicException(string topic)
        : base($"topic: '{topic}' is invalid.")
    {
    }
}
