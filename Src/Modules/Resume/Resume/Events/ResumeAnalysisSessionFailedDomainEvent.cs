using AI.Common.Core;
using Resume.Enums;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeAnalysisSessionFailedDomainEvent(ResumeId ResumeId, ResumeAnalysisFailureReason Reason) : IDomainEvent;
