using Resume.Exceptions;

namespace Resume.ValueObjects;

public record FileReference
{
    public string ResumeUrl { get; }
    public string FileName { get; }

    private FileReference(string resumeUrl, string fileName)
    {
        ResumeUrl = resumeUrl;
        FileName = fileName;
    }

    public static FileReference Of(string resumeUrl, string fileName)
    {
        if (string.IsNullOrEmpty(resumeUrl) || string.IsNullOrEmpty(fileName))
        {
            throw new SourceFileException(resumeUrl, fileName);
        }

        return new FileReference(resumeUrl, fileName);
    }

    public static implicit operator string(FileReference @value)
    {
        return @value.ResumeUrl;
    }
}