

using Resume.Exceptions;

namespace Resume.ValueObjects;

public record SkillId
{
    public Guid Value { get; }

    private SkillId(Guid value)
    {
        Value = value;
    }

    public static SkillId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new SkillIdException(value);
        }

        return new SkillId(value);
    }

    public static implicit operator Guid(SkillId id)
    {
        return id.Value;
    }
}