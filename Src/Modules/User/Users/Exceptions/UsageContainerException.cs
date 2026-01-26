using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UsageContainerException : DomainException
{
    public UsageContainerException(string usage_container)
        : base($"usage_container: '{usage_container}' is invalid.")
    {
    }
}
