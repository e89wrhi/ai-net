using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record LessonGeneratedDomainEvent(ProfileId ProfileId, LessonId LessonId, string Title) : IDomainEvent;
