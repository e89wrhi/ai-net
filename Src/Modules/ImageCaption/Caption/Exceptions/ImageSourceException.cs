using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class ImageSourceException : DomainException
{
    public ImageSourceException(string source)
        : base($"img_source: '{source}' is invalid.")
    {
    }
}
