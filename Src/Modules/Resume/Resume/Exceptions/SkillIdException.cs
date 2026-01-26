using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class SkillIdException : DomainException
{
    public SkillIdException(Guid skill_id)
        : base($"skill_id: '{skill_id}' is invalid.")
    {
    }
}
