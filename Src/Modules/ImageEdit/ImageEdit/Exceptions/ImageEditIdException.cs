using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class ImageEditIdException : DomainException
{
    public ImageEditIdException(Guid id)
        : base($"edit_id: '{id}' is invalid.")
    {
    }
}