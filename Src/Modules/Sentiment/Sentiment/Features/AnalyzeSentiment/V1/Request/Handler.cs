using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Sentiment.Data;
using Sentiment.Models;
using Sentiment.ValueObjects;

namespace Sentiment.Features.AnalyzeSentiment.V1;


internal class AnalyzeSentimentWithAIHandler : ICommandHandler<AnalyzeSentimentCommand, AnalyzeSentimentCommandResult>
{
    private readonly SentimentDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AnalyzeSentimentWithAIHandler(SentimentDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<AnalyzeSentimentCommandResult> Handle(AnalyzeSentimentCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a sentiment analysis expert. Analyze the sentiment of the following text. Return ONLY a single word (Positive, Negative, or Neutral) followed by a comma and a confidence score between 0 and 1. Example: Positive, 0.95"),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : request.Text)
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
        var responseText = chatCompletion.Messages[0].Text ?? "Neutral, 0.0";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Parse "Sentiment, Score"
        var parts = responseText.Split(',');
        var sentimentStr = parts[0].Trim();
        double score = 0.0;
        if (parts.Length > 1 && double.TryParse(parts[1].Trim(), out var parsedScore))
        {
            score = parsedScore;
        }

        // Persist
        var sessionId = SentimentId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TextSentimentConfiguration(
            Enums.SentimentDetailLevel.Basic,
            LanguageCode.Of("en"));

        var session = TextSentimentSession.Create(sessionId, userId, modelId, config);

        var resultId = SentimentResultId.Of(Guid.NewGuid());
        var sentimentVo = SentimentText.Of(sentimentStr);
        var scoreVo = SentimentScore.Of(score);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = TextSentimentResult.Create(
                resultId, 
                request.Text, 
                sentimentVo, 
                scoreVo, 
                tokenCountVo, 
                costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeSentimentCommandResult(sessionId.Value, resultId.Value, sentimentStr, score, modelIdStr, providerName);
    }
}
