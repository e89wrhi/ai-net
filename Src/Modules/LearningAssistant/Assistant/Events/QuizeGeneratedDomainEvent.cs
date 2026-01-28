using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record QuizeGeneratedDomainEvent(LessonId LessonId, QuizeId QuizeId, string Question) : IDomainEvent;
