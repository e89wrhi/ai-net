using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SourceFileException : DomainException
{
    public SourceFileException(string source_filee)
        : base($"source_filee: '{source_filee}' is invalid.")
    {
    }
}
