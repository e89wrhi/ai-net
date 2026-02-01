using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class ModuleIdException : DomainException
{
    public ModuleIdException(Guid id)
        : base($"module_id: '{id}' is invalid.")
    {
    }
}
