using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public record FileReference
{
    public string ImageUrl { get; }
    public string FileName { get; }

    private FileReference(string imageUrl, string fileName)
    {
        ImageUrl = imageUrl;
        FileName = fileName;
    }

    public static FileReference Of(string imageUrl, string fileName)
    {
        if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(fileName))
        {
            throw new FileReferenceException(imageUrl, fileName);
        }

        return new FileReference(imageUrl, fileName);
    }

    public static implicit operator string(FileReference @value)
    {
        return @value.ImageUrl;
    }
}