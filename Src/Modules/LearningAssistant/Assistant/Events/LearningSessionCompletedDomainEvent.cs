using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningSessionCompletedDomainEvent(LearningId Id) : IDomainEvent;
