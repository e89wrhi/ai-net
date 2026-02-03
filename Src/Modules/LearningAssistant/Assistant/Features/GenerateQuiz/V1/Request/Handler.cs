using AI.Common.Core;
using AiOrchestration.ValueObjects;
using LearningAssistant.Data;
using LearningAssistant.Models;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using LearningAssistant.ValueObjects;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateQuiz.V1;


internal class GenerateAIQuizHandler : ICommandHandler<GenerateAIQuizCommand, GenerateAIQuizCommandResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateAIQuizHandler(LearningDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAIQuizCommandResult> Handle(GenerateAIQuizCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a quiz master. Create a {request.QuestionCount} question quiz about {request.Topic}. Format it as JSON or clearly structured text.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, "Generate the quiz now.")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var quizText = completion.Messages[0].Text ?? "Failed to generate quiz.";

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "quiz-model");
        var config = LearningConfiguration.Of("assessment");

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(quizText);
        var outcomeVo = LearningOutcome.Of("Quiz Generated");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
        session.AddActivity(activity);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAIQuizCommandResult(sessionId.Value, activityId.Value, quizText);
    }
}
