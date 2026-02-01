using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record ProgrammingLanguage
{
    public string Value { get; }

    private ProgrammingLanguage(string value)
    {
        Value = value;
    }

    public static ProgrammingLanguage Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ProgrammingLanguageException(value);
        }

        return new ProgrammingLanguage(value);
    }

    public static implicit operator string(ProgrammingLanguage @value)
    {
        return @value.Value;
    }
}