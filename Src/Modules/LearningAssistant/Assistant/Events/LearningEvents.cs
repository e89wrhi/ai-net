using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LearningProfileCreatedDomainEvent(ProfileId ProfileId, string UserId) : IDomainEvent;
public record LessonGeneratedDomainEvent(ProfileId ProfileId, LessonId LessonId, string Title) : IDomainEvent;
public record QuizeCompletedDomainEvent(LessonId LessonId, QuizeId QuizeId, double Score) : IDomainEvent;
public record ProgressUpdatedDomainEvent(ProfileId ProfileId, double NewCompletionRate) : IDomainEvent;
