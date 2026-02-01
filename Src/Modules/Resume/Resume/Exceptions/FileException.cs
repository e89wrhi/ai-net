using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class FileException : DomainException
{
    public FileException(string file)
        : base($"file: '{file}' is invalid.")
    {
    }
}
