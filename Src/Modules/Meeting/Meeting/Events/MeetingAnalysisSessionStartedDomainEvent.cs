using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingAnalysisSessionStartedDomainEvent(MeetingId MeetingId, UserId UserId, ModelId AiModel) : IDomainEvent;
