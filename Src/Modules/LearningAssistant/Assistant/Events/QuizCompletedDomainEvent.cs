using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record QuizCompletedDomainEvent(LessonId LessonId, QuizId QuizId, double Score) : IDomainEvent;
