using AI.Common.Core;
using AiOrchestration.ValueObjects;
using LearningAssistant.Data;
using LearningAssistant.Models;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using LearningAssistant.ValueObjects;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateLesson.V1;


internal class GenerateAILessonHandler : ICommandHandler<GenerateLessonCommand, GenerateLessonCommandResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateAILessonHandler(LearningDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateLessonCommandResult> Handle(GenerateLessonCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a professional teacher. Generate a comprehensive lesson about the topic provided for a {request.Level} level student.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Explain: {request.Topic}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var contentText = completion.Messages[0].Text ?? "Failed to generate lesson.";

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "learning-model");
        var config = LearningConfiguration.Of("standard");

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(contentText);
        var outcomeVo = LearningOutcome.Of("Lesson Generated");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
        session.AddActivity(activity);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAILessonCommandResult(sessionId.Value, activityId.Value, contentText);
    }
}
