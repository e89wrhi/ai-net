using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record ActivityId
{
    public Guid Value { get; }

    private ActivityId(Guid value)
    {
        Value = value;
    }

    public static ActivityId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ActivityIdException(value);
        }

        return new ActivityId(value);
    }

    public static implicit operator Guid(ActivityId id)
    {
        return id.Value;
    }
}