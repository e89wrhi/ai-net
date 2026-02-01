using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public class LearningContent
{
    public string Value { get; }

    private LearningContent(string value)
    {
        Value = value;
    }

    public static LearningContent Of(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ContentException(value);
        }

        return new LearningContent(value);
    }

    public static implicit operator string(LearningContent @value)
    {
        return @value.Value;
    }
}