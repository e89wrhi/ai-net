using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record CodeGenId
{
    public Guid Value { get; }

    private CodeGenId(Guid value)
    {
        Value = value;
    }

    public static CodeGenId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new CodeGenIdException(value);
        }

        return new CodeGenId(value);
    }

    public static implicit operator Guid(CodeGenId id)
    {
        return id.Value;
    }
}