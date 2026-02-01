using Translate.Exceptions;

namespace Translate.ValueObjects;

public record TranslateId
{
    public Guid Value { get; }

    private TranslateId(Guid value)
    {
        Value = value;
    }

    public static TranslateId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new TranslateIdException(value);
        }

        return new TranslateId(value);
    }

    public static implicit operator Guid(TranslateId id)
    {
        return id.Value;
    }
}