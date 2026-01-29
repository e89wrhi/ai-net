using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using LearningAssistant.Events;
using LearningAssistant.Features.GenerateLesson.V1;
using LearningAssistant.Features.GenerateQuiz.V1;
using LearningAssistant.Features.SubmitQuiz.V1;

namespace LearningAssistant;

public sealed class AssistantEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            LearningProfileCreatedDomainEvent e => new LearningProfileCreated(e.ProfileId.Value),
            LessonGeneratedDomainEvent e => new LessonGenerated(e.LessonId.Value),
            QuizCompletedDomainEvent e => new QuizCompleted(e.QuizId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            LessonGeneratedDomainEvent e => new GenerateLessonMongo(e.ProfileId.Value, e.LessonId.Value, e.Title, e.Content),
            QuizGeneratedDomainEvent e => new GenerateQuizMongo(e.LessonId.Value, e.QuizId.Value, e.Questions, "Wait for answer"),
            QuizCompletedDomainEvent e => new SubmitQuizMongo(e.QuizId.Value, "User Answer", true), // Simplified for mapper
            _ => null
        };
    }
}