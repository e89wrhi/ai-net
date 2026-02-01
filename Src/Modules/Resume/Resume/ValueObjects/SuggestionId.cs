using Resume.Exceptions;

namespace Resume.ValueObjects;

public record SuggestionId
{
    public Guid Value { get; }

    private SuggestionId(Guid value)
    {
        Value = value;
    }

    public static SuggestionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SuggestionIdException(value);
        }

        return new SuggestionId(value);
    }

    public static implicit operator Guid(SuggestionId id)
    {
        return id.Value;
    }
}