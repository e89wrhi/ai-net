using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class CaptionException : DomainException
{
    public CaptionException(string caption)
        : base($"caption: '{caption}' is invalid.")
    {
    }
}
