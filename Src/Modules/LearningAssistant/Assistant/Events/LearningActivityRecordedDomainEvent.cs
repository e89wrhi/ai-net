using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningActivityRecordedDomainEvent(LearningId Id, ActivityId ActivityId, string Topic) : IDomainEvent;
