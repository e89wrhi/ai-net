using AI.Common.BaseExceptions;

namespace ImageCaption.Exceptions;

public class FileReferenceException : DomainException
{
    public FileReferenceException(string imageurl, string filename)
        : base($"image: '{imageurl}'/file: {filename} is invalid.")
    {
    }
}
