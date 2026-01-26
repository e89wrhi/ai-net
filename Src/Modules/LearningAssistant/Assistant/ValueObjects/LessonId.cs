

using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record LessonId
{
    public Guid Value { get; }

    private LessonId(Guid value)
    {
        Value = value;
    }

    public static LessonId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new LessonIdException(value);
        }

        return new LessonId(value);
    }

    public static implicit operator Guid(LessonId id)
    {
        return id.Value;
    }
}