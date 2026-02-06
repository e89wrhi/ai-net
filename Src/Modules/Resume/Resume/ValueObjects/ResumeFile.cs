using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ResumeFile
{
    public string ResumeUrl { get; }
    public string FileName { get; }

    private ResumeFile(string resumeUrl, string fileName)
    {
        ResumeUrl = resumeUrl;
        FileName = fileName;
    }

    public static ResumeFile Of(string resumeUrl, string fileName)
    {
        if (string.IsNullOrEmpty(resumeUrl) || string.IsNullOrEmpty(fileName))
        {
            throw new FileException(resumeUrl);
        }

        return new ResumeFile(resumeUrl, fileName);
    }

    public static implicit operator string(ResumeFile @value)
    {
        return @value.ResumeUrl;
    }
}