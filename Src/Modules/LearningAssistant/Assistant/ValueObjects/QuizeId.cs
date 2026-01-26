

using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record QuizeId
{
    public Guid Value { get; }

    private QuizeId(Guid value)
    {
        Value = value;
    }

    public static QuizeId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new QuizeIdException(value);
        }

        return new QuizeId(value);
    }

    public static implicit operator Guid(QuizeId id)
    {
        return id.Value;
    }
}