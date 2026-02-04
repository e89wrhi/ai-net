using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateLesson.V1;


internal class GenerateAILessonHandler : ICommandHandler<GenerateLessonCommand, GenerateLessonCommandResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateAILessonHandler(LearningDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateLessonCommandResult> Handle(GenerateLessonCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: $"You are a professional teacher. Generate a comprehensive lesson about the topic provided for a {request.DifficultyLevel} level student."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Explain: {request.Topic}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? "Failed to generate lesson.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new LearningConfiguration(
            request.Mode,
            request.DifficultyLevel);

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(responseText);
        var outcomeVo = LearningOutcome.Of("Lesson Generated");
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var activity = LearningActivity.Create(
                    activityId, 
                    topicVo, 
                    contentVo, 
                    outcomeVo, 
                    tokenCountVo, 
                    costVo);

        session.AddActivity(activity);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateLessonCommandResult(sessionId.Value, activityId.Value, responseText, modelIdStr, providerName);
    }
}
