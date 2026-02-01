using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record CodeDebugId
{
    public Guid Value { get; }

    private CodeDebugId(Guid value)
    {
        Value = value;
    }

    public static CodeDebugId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new CodeDebugIdException(value);
        }

        return new CodeDebugId(value);
    }

    public static implicit operator Guid(CodeDebugId id)
    {
        return id.Value;
    }
}