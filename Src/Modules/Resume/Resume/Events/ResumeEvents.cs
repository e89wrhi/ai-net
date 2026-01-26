using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeUploadedDomainEvent(ResumeId ResumeId, string UserId, string FileName) : IDomainEvent;
public record ResumeParsedDomainEvent(ResumeId ResumeId, int TextLength) : IDomainEvent;
public record ResumeAnalyzedDomainEvent(ResumeId ResumeId, int SkillCount, int SuggestionCount) : IDomainEvent;
public record ResumeAnalysisFailedDomainEvent(ResumeId ResumeId, string Reason) : IDomainEvent;
