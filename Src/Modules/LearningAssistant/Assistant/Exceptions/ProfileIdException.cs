using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class ProfileIdException : DomainException
{
    public ProfileIdException(Guid profile_id)
        : base($"profile_id: '{profile_id}' is invalid.")
    {
    }
}
