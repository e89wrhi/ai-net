using AI.Common.Core;
using LearningAssistant.ValueObjects;

namespace LearningAssistant.Events;

public record QuizGeneratedDomainEvent(LessonId LessonId, QuizId QuizId, string Questions) : IDomainEvent;
