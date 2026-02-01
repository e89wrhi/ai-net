using AI.Common.Core;
using AiOrchestration.ValueObjects;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningSessionStartedDomainEvent(LearningId Id, UserId UserId, ModelId AiModel) : IDomainEvent;
