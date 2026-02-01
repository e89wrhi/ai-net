using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UsageModuleIdException : DomainException
{
    public UsageModuleIdException(Guid id)
        : base($"usage_module_id: '{id}' is invalid.")
    {
    }
}
