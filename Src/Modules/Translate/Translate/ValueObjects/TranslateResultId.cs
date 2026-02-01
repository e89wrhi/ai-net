using Translate.Exceptions;

namespace Translate.ValueObjects;

public record TranslateResultId
{
    public Guid Value { get; }

    private TranslateResultId(Guid value)
    {
        Value = value;
    }

    public static TranslateResultId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResultIdException(value);
        }

        return new TranslateResultId(value);
    }

    public static implicit operator Guid(TranslateResultId id)
    {
        return id.Value;
    }
}