using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record CodeGenerationPrompt
{
    public string Value { get; }

    private CodeGenerationPrompt(string value)
    {
        Value = value;
    }

    public static CodeGenerationPrompt Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new CodeGenPromptException(value);
        }

        return new CodeGenerationPrompt(value);
    }

    public static implicit operator string(CodeGenerationPrompt @value)
    {
        return @value.Value;
    }
}