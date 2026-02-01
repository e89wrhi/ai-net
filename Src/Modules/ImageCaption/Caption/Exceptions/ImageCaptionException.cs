using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ImageCaptionException : DomainException
{
    public ImageCaptionException(string caption)
        : base($"caption: '{caption}' is invalid.")
    {
    }
}
