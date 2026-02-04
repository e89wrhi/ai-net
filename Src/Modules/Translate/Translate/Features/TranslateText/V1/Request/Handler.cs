using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;

namespace Translate.Features.TranslateText.V1;


internal class TranslateTextWithAIHandler : ICommandHandler<TranslateTextCommand, TranslateTextCommandResult>
{
    private readonly TranslateDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public TranslateTextWithAIHandler(TranslateDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<TranslateTextCommandResult> Handle(TranslateTextCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                role: ChatRole.System, 
                content: "You are a professional translator."),
            new ChatMessage(
                role: ChatRole.User, 
                content: $"Translate the following text from {request.SourceLanguage} to {request.TargetLanguage}. Detail level: {request.DetailLevel}.\n\nText: {request.Text}")
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
        
        var translatedText = chatCompletion.Messages[0].Text ?? "Translation failed.";
        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + translatedText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;


        // Persist
        var sessionId = TranslateId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TranslationConfiguration(
            LanguageCode.Of(request.SourceLanguage),
            LanguageCode.Of(request.TargetLanguage),
            request.DetailLevel);

        var session = TranslationSession.Create(sessionId, userId, modelId, config);

        var resultId = TranslateResultId.Of(Guid.NewGuid());
        var translatedTextVo = TranslatedText.Of(translatedText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = TranslationResult.Create(
                resultId, 
                request.Text, 
                translatedTextVo, 
                tokenCountVo, 
                costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TranslateTextCommandResult(sessionId.Value, resultId.Value, translatedText, modelIdStr, providerName);
    }
}
