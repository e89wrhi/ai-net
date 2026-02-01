using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record LearningId
{
    public Guid Value { get; }

    private LearningId(Guid value)
    {
        Value = value;
    }

    public static LearningId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new LearningIdException(value);
        }

        return new LearningId(value);
    }

    public static implicit operator Guid(LearningId id)
    {
        return id.Value;
    }
}