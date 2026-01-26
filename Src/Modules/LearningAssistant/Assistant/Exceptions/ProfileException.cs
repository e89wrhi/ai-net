using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class ProfileNotFoundException : DomainException
{
    public ProfileNotFoundException(Guid profileId)
        : base($"profile: '{profileId}' not found.")
    {
    }
}

public class ProfileAlreadyExistException : DomainException
{
    public ProfileAlreadyExistException(Guid profileId)
        : base($"profile: '{profileId}' already exist.")
    {
    }
}
