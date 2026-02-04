using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateQuiz.V1;


internal class GenerateAIQuizHandler : ICommandHandler<GenerateQuizCommand, GenerateQuizCommandResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateAIQuizHandler(LearningDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<GenerateQuizCommandResult> Handle(GenerateQuizCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: $"You are a quiz master. Create a {request.QuestionCount} question quiz about {request.Topic}. Format it as JSON or clearly structured text."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : "Generate the quiz now.")
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
        var responseText = chatCompletion.Messages[0].Text ?? "Failed to generate quiz.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new LearningConfiguration(
            mode: Enums.LearningMode.Quiz,
            difficulty: Enums.DifficultyLevel.Medium);

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(responseText);
        var outcomeVo = LearningOutcome.Of("Quiz Generated");
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

        return new GenerateQuizCommandResult(sessionId.Value, activityId.Value, responseText, modelIdStr, providerName);
    }
}
