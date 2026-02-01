using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class EditedImageException : DomainException
{
    public EditedImageException(string image)
        : base($"edited_image: '{image}' is invalid.")
    {
    }
}