using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record QuizeCompletedDomainEvent(LessonId LessonId, QuizeId QuizeId, double Score) : IDomainEvent;
