
using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record QuizId
{
    public Guid Value { get; }

    private QuizId(Guid value)
    {
        Value = value;
    }

    public static QuizId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new QuizIdException(value);
        }

        return new QuizId(value);
    }

    public static implicit operator Guid(QuizId id)
    {
        return id.Value;
    }
}