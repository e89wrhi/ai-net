using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ObjectIdException : DomainException
{
    public ObjectIdException(Guid object_id)
        : base($"object_id: '{object_id}' is invalid.")
    {
    }
}
