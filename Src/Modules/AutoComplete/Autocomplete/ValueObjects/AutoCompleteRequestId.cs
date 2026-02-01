using AutoComplete.Exceptions;

namespace AutoComplete.ValueObjects;

public record AutoCompleteRequestId
{
    public Guid Value { get; }

    private AutoCompleteRequestId(Guid value)
    {
        Value = value;
    }

    public static AutoCompleteRequestId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new AutoCompleteRequestIdException(value);
        }

        return new AutoCompleteRequestId(value);
    }

    public static implicit operator Guid(AutoCompleteRequestId id)
    {
        return id.Value;
    }
}