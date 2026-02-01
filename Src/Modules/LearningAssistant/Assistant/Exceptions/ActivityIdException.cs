using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class ActivityIdException : DomainException
{
    public ActivityIdException(Guid quiz_id)
        : base($"quiz_id: '{quiz_id}' is invalid.")
    {
    }
}
