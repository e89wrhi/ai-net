using AiOrchestration.ValueObjects;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Runtime.CompilerServices;
using System.Text;

namespace LearningAssistant.Features.StreamLesson.V1;

internal class StreamAILessonHandler : IStreamRequestHandler<StreamAILessonCommand, string>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamAILessonHandler(LearningDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamAILessonCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a professional teacher. Generate a comprehensive lesson about the topic provided for a {request.Level} level student.";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Explain: {request.Topic}")
        };

        var fullContentBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullContentBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist interaction
        await PersistActivityAsync(request, fullContentBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistActivityAsync(StreamAILessonCommand request, string fullContent, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = LearningId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "learning-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new LearningConfiguration(
                mode: LearningMode.Lesson,
                difficulty: request.Level);

            var session = LearningSession.Create(sessionId, userId, modelId, config);

            var activityId = ActivityId.Of(Guid.NewGuid());
            var topicVo = LearningTopic.Of(request.Topic);
            var contentVo = LearningContent.Of(fullContent);
            var outcomeVo = LearningOutcome.Of("Streamed Lesson Completed");
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
            session.AddActivity(activity);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}


