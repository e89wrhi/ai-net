using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeAnalysisSessionStartedDomainEvent(ResumeId ResumeId, UserId UserId, ModelId AiModel) : IDomainEvent;
