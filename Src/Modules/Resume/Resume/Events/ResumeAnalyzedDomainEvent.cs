using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeAnalyzedDomainEvent(ResumeId ResumeId, int SkillCount, int SuggestionCount) : IDomainEvent;
