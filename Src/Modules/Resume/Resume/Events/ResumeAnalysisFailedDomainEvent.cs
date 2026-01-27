using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeAnalysisFailedDomainEvent(ResumeId ResumeId, string Reason) : IDomainEvent;
