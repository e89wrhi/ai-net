using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record GeneratedCode
{
    public string Value { get; }

    private GeneratedCode(string value)
    {
        Value = value;
    }

    public static GeneratedCode Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new GeneratedCodeException(value);
        }

        return new GeneratedCode(value);
    }

    public static implicit operator string(GeneratedCode @value)
    {
        return @value.Value;
    }
}