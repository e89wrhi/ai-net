using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SourceFileException : DomainException
{
    public SourceFileException(string resumeurl, string filename)
        : base($"resume: '{resumeurl}'/file: {filename} is invalid.")
    {
    }
}
