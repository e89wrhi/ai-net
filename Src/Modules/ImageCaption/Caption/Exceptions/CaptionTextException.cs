using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class CaptionTextException : DomainException
{
    public CaptionTextException(string text)
        : base($"caption_text: '{text} is invalid.")
    {
    }
}
