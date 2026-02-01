using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class ImageSourceException : DomainException
{
    public ImageSourceException(string source)
        : base($"img_source: '{source}' is invalid.")
    {
    }
}
