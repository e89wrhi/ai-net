using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeAnalysisSessionCompletedDomainEvent(ResumeId ResumeId) : IDomainEvent;
