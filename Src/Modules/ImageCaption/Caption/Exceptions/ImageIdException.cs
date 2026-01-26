using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ImageIdException : DomainException
{
    public ImageIdException(Guid image_idd)
        : base($"image_idd: '{image_idd}' is invalid.")
    {
    }
}
