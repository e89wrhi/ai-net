using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ObjectException : DomainException
{
    public ObjectException(string object_)
        : base($"object_: '{object_}' is invalid.")
    {
    }
}
