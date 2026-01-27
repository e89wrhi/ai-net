using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record ProgressUpdatedDomainEvent(ProfileId ProfileId, double NewCompletionRate) : IDomainEvent;
