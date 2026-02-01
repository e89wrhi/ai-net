using ImageCaption.Exceptions;

namespace ImageCaption.ValueObjects;

public class CaptionText
{
    public string Value { get; }

    private CaptionText(string value)
    {
        Value = value;
    }

    public static CaptionText Of(string value)
    {
        if (value == string.Empty)
        {
            throw new CaptionTextException(value);
        }

        return new CaptionText(value);
    }

    public static implicit operator string(CaptionText id)
    {
        return id.Value;
    }
}