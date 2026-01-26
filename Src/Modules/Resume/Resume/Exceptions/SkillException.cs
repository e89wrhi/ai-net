using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SkillException : DomainException
{
    public SkillException(string skill)
        : base($"skill: '{skill}' is invalid.")
    {
    }
}
