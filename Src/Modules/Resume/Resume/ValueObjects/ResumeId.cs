

using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ResumeId
{
    public Guid Value { get; }

    private ResumeId(Guid value)
    {
        Value = value;
    }

    public static ResumeId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResumeIdException(value);
        }

        return new ResumeId(value);
    }

    public static implicit operator Guid(ResumeId id)
    {
        return id.Value;
    }
}