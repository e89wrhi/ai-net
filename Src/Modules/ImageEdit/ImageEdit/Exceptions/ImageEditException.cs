using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class ImageEditNotFoundException : DomainException
{
    public ImageEditNotFoundException(Guid imageeditId)
        : base($"imageedit: '{imageeditId}' not found.")
    {
    }
}

public class ImageEditAlreadyExistException : DomainException
{
    public ImageEditAlreadyExistException(Guid imageeditId)
        : base($"imageedit: '{imageeditId}' already exist.")
    {
    }
}
