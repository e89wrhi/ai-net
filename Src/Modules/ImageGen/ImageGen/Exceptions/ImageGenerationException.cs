using AI.Common.BaseExceptions;

namespace ImageGen.Exceptions;

public class ImageGenNotFoundException : DomainException
{
    public ImageGenNotFoundException(Guid imagegenId)
        : base($"imagegen: '{imagegenId}' not found.")
    {
    }
}

public class ImageGenAlreadyExistException : DomainException
{
    public ImageGenAlreadyExistException(Guid imagegenId)
        : base($"imagegen: '{imagegenId}' already exist.")
    {
    }
}
