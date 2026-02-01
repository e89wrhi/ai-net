using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record SourceCode
{
    public string Value { get; }

    private SourceCode(string value)
    {
        Value = value;
    }

    public static SourceCode Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new SourceCodeException(value);
        }

        return new SourceCode(value);
    }

    public static implicit operator string(SourceCode @value)
    {
        return @value.Value;
    }
}