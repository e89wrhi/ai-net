using Resume.Exceptions;

namespace Resume.ValueObjects;

public record ResumeAnalysisConfiguration
{
    public bool IncludeSkills { get; }
    public bool IncludeExperience { get; }
    public bool IncludeEducation { get; }

    public ResumeAnalysisConfiguration(bool includeSkills, bool includeExperience, bool includeEducation)
    {
        IncludeSkills = includeSkills;
        IncludeExperience = includeExperience;
        IncludeEducation = includeEducation;
    }
}
