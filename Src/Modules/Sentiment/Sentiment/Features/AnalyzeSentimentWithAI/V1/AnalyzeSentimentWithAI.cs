using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Sentiment.Data;
using Sentiment.Models;
using Sentiment.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Sentiment.Features.AnalyzeSentimentWithAI.V1;

public record AnalyzeSentimentWithAICommand(string Text) : ICommand<AnalyzeSentimentWithAIResult>;

public record AnalyzeSentimentWithAIResult(Guid SessionId, Guid ResultId, string Sentiment, double Score);

public class AnalyzeSentimentWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/sentiment/analyze",
                async (AnalyzeSentimentWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeSentimentWithAICommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeSentimentWithAIResponseDto(result.SessionId, result.ResultId, result.Sentiment, result.Score));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeSentimentWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<AnalyzeSentimentWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Sentiment with AI")
            .WithDescription("Uses AI to analyze the sentiment of the provided text, returning sentiment type and confidence score.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeSentimentWithAIRequestDto(string Text);
public record AnalyzeSentimentWithAIResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score);

internal class AnalyzeSentimentWithAIHandler : ICommandHandler<AnalyzeSentimentWithAICommand, AnalyzeSentimentWithAIResult>
{
    private readonly SentimentDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeSentimentWithAIHandler(SentimentDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeSentimentWithAIResult> Handle(AnalyzeSentimentWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var systemPrompt = "You are a sentiment analysis expert. Analyze the sentiment of the following text. Return ONLY a single word (Positive, Negative, or Neutral) followed by a comma and a confidence score between 0 and 1. Example: Positive, 0.95";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Text)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Message.Text ?? "Neutral, 0.0";
        
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

        return new AnalyzeSentimentWithAIResult(sessionId.Value, resultId.Value, sentimentStr, score);
    }
}
