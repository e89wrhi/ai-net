using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using LearningAssistant.Events;
using LearningAssistant.Features.GenerateLesson.V1;
using LearningAssistant.Features.GenerateQuize.V1;
using LearningAssistant.Features.SubmitQuize.V1;

namespace LearningAssistant;

public sealed class AssistantEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            LearningProfileCreatedDomainEvent e => new LearningProfileCreated(e.ProfileId.Value),
            LessonGeneratedDomainEvent e => new LessonGenerated(e.LessonId.Value),
            QuizeCompletedDomainEvent e => new QuizeCompleted(e.QuizeId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            LessonGeneratedDomainEvent e => new GenerateLessonMongo(e.ProfileId.Value, e.LessonId.Value, e.Title, e.Content),
            QuizeGeneratedDomainEvent e => new GenerateQuizeMongo(e.LessonId.Value, e.QuizeId.Value, e.Question, "Wait for answer"),
            QuizeCompletedDomainEvent e => new SubmitQuizeMongo(e.QuizeId.Value, "User Answer", true), // Simplified for mapper
            _ => null
        };
    }
}