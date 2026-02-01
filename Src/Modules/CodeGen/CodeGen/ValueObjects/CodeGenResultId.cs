using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record CodeGenResultId
{
    public Guid Value { get; }

    private CodeGenResultId(Guid value)
    {
        Value = value;
    }

    public static CodeGenResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new CodeGenResultId(value);
    }

    public static implicit operator Guid(CodeGenResultId id)
    {
        return id.Value;
    }
}