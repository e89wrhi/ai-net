using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Exceptions;
using LearningAssistant.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.SubmitQuiz.V1;


internal class SubmitQuizHandler : IRequestHandler<SubmitQuizCommand, SubmitQuizCommandResponse>
{
    private readonly LearningDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public SubmitQuizHandler(LearningDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<SubmitQuizCommandResponse> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        string submitPrompt = "";
        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System,
                    content: ""),
             new ChatMessage(
                    role: ChatRole.User,
                    content: submitPrompt)
        };
        #endregion

        Guard.Against.Null(request, nameof(request));

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = "" };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        var lesson = await _dbContext.Sessions
            .FirstOrDefaultAsync(x => x.Id == LearningId.Of(request.LessonId), cancellationToken);

        if (lesson == null)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        var profile = await _dbContext.Sessions
            .Include(x => x.Activities)
            .FirstOrDefaultAsync(x => x.Id == lesson.Id, cancellationToken);

        if (profile == null)
        {
            throw new LearningNotFoundException(lesson.Id);
        }

        profile.SubmitScore(request.Score);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitQuizCommandResponse(request.QuizId);
    }
}
