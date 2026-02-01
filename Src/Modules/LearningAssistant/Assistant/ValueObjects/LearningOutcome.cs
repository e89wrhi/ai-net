using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public class LearningOutcome
{
    public string Value { get; }

    private LearningOutcome(string value)
    {
        Value = value;
    }

    public static LearningOutcome Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new OutcomeException(value);
        }

        return new LearningOutcome(value);
    }

    public static implicit operator string(LearningOutcome @value)
    {
        return @value.Value;
    }
}