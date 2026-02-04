using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Sentiment.Data;
using Sentiment.Models;
using Sentiment.ValueObjects;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;


internal class AnalyzeSentimentDetailedHandler : ICommandHandler<AnalyzeSentimentDetailedCommand, AnalyzeSentimentDetailedCommandResult>
{
    private readonly SentimentDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AnalyzeSentimentDetailedHandler(SentimentDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeSentimentDetailedCommandResult> Handle(AnalyzeSentimentDetailedCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a sentiment analyst. Provide a detailed analysis of the following text. You must return your response in JSON format with three fields: 'sentiment' (Positive, Negative, or Neutral), 'score' (0.0 to 1.0), and 'explanation' (a brief sentence explaining the reasoning)."),
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
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? 
            (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        var responseJson = chatCompletion.Messages[0].Text ?? "{\"sentiment\": \"Neutral\", \"score\": 0.0, \"explanation\": \"Unparseable response\"}";

        // Very crude JSON parsing for demonstration, in a real app use a JSON serializer
        string sentiment = "Neutral";
        double score = 0.0;
        string explanation = "Detailed analysis results.";

        try
        {
            // Simulated parsing
            if (responseJson.Contains("\"sentiment\":")) sentiment = responseJson.Split("\"sentiment\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"score\":"))
            {
                var scorePart = responseJson.Split("\"score\":")[1].Split(",")[0].Split("}")[0].Trim();
                double.TryParse(scorePart, out score);
            }
            if (responseJson.Contains("\"explanation\":")) explanation = responseJson.Split("\"explanation\":")[1].Split("\"")[1];
        }
        catch { }

        // Persist
        var sessionId = SentimentId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TextSentimentConfiguration(
            detailLevel: request.DetailLevel, 
            language: LanguageCode.Of(request.Language));

        var session = TextSentimentSession.Create(sessionId, userId, modelId, config);

        var resultId = SentimentResultId.Of(Guid.NewGuid());
        var sentimentVo = SentimentText.Of(sentiment);
        var scoreVo = SentimentScore.Of(score);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = TextSentimentResult.Create(
                resultId, 
                request.Text, 
                sentimentVo, 
                scoreVo, 
                tokenCountVo, costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeSentimentDetailedCommandResult(sessionId.Value, resultId.Value, 
            sentiment, score, explanation, modelIdStr, providerName);
    }
}
