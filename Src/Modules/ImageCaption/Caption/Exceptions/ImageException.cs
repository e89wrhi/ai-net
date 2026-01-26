using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ImageNotFoundException : DomainException
{
    public ImageNotFoundException(Guid imageId)
        : base($"image: '{imageId}' not found.")
    {
    }
}

public class ImageAlreadyExistException : DomainException
{
    public ImageAlreadyExistException(Guid imageId)
        : base($"image: '{imageId}' already exist.")
    {
    }
}

