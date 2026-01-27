using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningProfileCreatedDomainEvent(ProfileId ProfileId, string UserId) : IDomainEvent;
