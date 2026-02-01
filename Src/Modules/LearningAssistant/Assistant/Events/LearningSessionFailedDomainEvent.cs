using AI.Common.Core;
using LearningAssistant.Enums;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningSessionFailedDomainEvent(LearningId Id, LearningFailureReason Reason) : IDomainEvent;
