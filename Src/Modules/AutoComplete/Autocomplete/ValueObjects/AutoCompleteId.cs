using AutoComplete.Exceptions;

namespace AutoComplete.ValueObjects;

public record AutoCompleteId
{
    public Guid Value { get; }

    private AutoCompleteId(Guid value)
    {
        Value = value;
    }

    public static AutoCompleteId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new AutoCompleteIdException(value);
        }

        return new AutoCompleteId(value);
    }

    public static implicit operator Guid(AutoCompleteId id)
    {
        return id.Value;
    }
}