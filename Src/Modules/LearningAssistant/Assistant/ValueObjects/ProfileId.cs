

using LearningAssistant.Exceptions;

namespace LearningAssistant.ValueObjects;

public record ProfileId
{
    public Guid Value { get; }

    private ProfileId(Guid value)
    {
        Value = value;
    }

    public static ProfileId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ProfileIdException(value);
        }

        return new ProfileId(value);
    }

    public static implicit operator Guid(ProfileId id)
    {
        return id.Value;
    }
}