using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public class LearningTopic
{
    public string Value { get; }

    private LearningTopic(string value)
    {
        Value = value;
    }

    public static LearningTopic Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new TopicException(value);
        }

        return new LearningTopic(value);
    }

    public static implicit operator string(LearningTopic @value)
    {
        return @value.Value;
    }
}