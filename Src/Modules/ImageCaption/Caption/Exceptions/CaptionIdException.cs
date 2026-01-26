using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class CaptionIdException : DomainException
{
    public CaptionIdException(Guid caption_id)
        : base($"caption_id: '{caption_id}' is invalid.")
    {
    }
}
