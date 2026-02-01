using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Summary.ValueObjects;

namespace Summary.Events;

public record TextSummarySessionStartedDomainEvent(SummaryId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
