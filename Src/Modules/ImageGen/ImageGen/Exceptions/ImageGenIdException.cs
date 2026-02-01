using AI.Common.BaseExceptions;

namespace ImageGen.Exceptions;

public class ImageGenIdException : DomainException
{
    public ImageGenIdException(Guid id)
        : base($"imagegen_id: '{id}' is invalid.")
    {
    }
}