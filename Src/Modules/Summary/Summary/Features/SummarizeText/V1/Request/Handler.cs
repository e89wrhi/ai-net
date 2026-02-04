using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;

namespace Summary.Features.SummarizeText.V1;


internal class SummarizeTextWithAIHandler : ICommandHandler<SummarizeTextCommand, SummarizeTextCommandResult>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public SummarizeTextWithAIHandler(SummaryDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<SummarizeTextCommandResult> Handle(SummarizeTextCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a professional summarization assistant."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

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
        var responseText = chatCompletion.Messages[0].Text ?? "Summary generation failed.";
        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;


        // Persist
        var sessionId = SummaryId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

        var session = TextSummarySession.Create(sessionId, userId, modelId, config);

        var resultId = SummaryResultId.Of(Guid.NewGuid());
        var summaryVo = SummaryText.Of(responseText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = TextSummaryResult.Create(
                resultId, 
                request.Text, 
                summaryVo, 
                tokenCountVo, 
                costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SummarizeTextCommandResult(sessionId.Value, resultId.Value, responseText, modelIdStr, providerName);
    }
}
