using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class FileReferenceException : DomainException
{
    public FileReferenceException(string file_reference)
        : base($"file_reference: '{file_reference}' is invalid.")
    {
    }
}
