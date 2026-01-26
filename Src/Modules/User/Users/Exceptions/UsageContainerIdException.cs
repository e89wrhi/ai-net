using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UsageContainerIdException : DomainException
{
    public UsageContainerIdException(Guid usage_container_id)
        : base($"usage_container_id: '{usage_container_id}' is invalid.")
    {
    }
}
