using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using Sentiment.Data;
using Sentiment.Models;
using Sentiment.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace Sentiment.Features.AnalyzeSentiment.V1;


internal class AnalyzeSentimentWithAIHandler : ICommandHandler<AnalyzeSentimentWithAICommand, AnalyzeSentimentWithAICommandResult>
{
    private readonly SentimentDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public AnalyzeSentimentWithAIHandler(SentimentDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeSentimentWithAICommandResult> Handle(AnalyzeSentimentWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var systemPrompt = "You are a sentiment analysis expert. Analyze the sentiment of the following text. Return ONLY a single word (Positive, Negative, or Neutral) followed by a comma and a confidence score between 0 and 1. Example: Positive, 0.95";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Text)
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Messages[0].Text ?? "Neutral, 0.0";

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
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "sentiment-model");
        var config = TextSentimentConfiguration.Of(LanguageCode.Of("en"));

        var session = TextSentimentSession.Create(sessionId, userId, modelId, config);

        var resultId = SentimentResultId.Of(Guid.NewGuid());
        var sentimentVo = SentimentText.Of(sentimentStr);
        var scoreVo = SentimentScore.Of(score);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = TextSentimentResult.Create(resultId, request.Text, sentimentVo, scoreVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeSentimentWithAICommandResult(sessionId.Value, resultId.Value, sentimentStr, score);
    }
}
