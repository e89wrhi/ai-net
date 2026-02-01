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

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;

public record AnalyzeSentimentDetailedCommand(string Text) : ICommand<AnalyzeSentimentDetailedResult>;

public record AnalyzeSentimentDetailedResult(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation);

public class AnalyzeSentimentDetailedEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/sentiment/analyze-detailed",
                async (AnalyzeSentimentDetailedRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeSentimentDetailedCommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeSentimentDetailedResponseDto(result.SessionId, result.ResultId, result.Sentiment, result.Score, result.Explanation));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeSentimentDetailed")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<AnalyzeSentimentDetailedResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Detailed Sentiment Analysis")
            .WithDescription("Uses AI to provide a deep sentiment analysis of the text, including an explanation of why the sentiment was chosen.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeSentimentDetailedRequestDto(string Text);
public record AnalyzeSentimentDetailedResponseDto(Guid SessionId, Guid ResultId, string Sentiment, double Score, string Explanation);

internal class AnalyzeSentimentDetailedHandler : ICommandHandler<AnalyzeSentimentDetailedCommand, AnalyzeSentimentDetailedResult>
{
    private readonly SentimentDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeSentimentDetailedHandler(SentimentDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeSentimentDetailedResult> Handle(AnalyzeSentimentDetailedCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var systemPrompt = "You are a sentiment analyst. Provide a detailed analysis of the following text. You must return your response in JSON format with three fields: 'sentiment' (Positive, Negative, or Neutral), 'score' (0.0 to 1.0), and 'explanation' (a brief sentence explaining the reasoning).";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Text)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseJson = completion.Message.Text ?? "{\"sentiment\": \"Neutral\", \"score\": 0.0, \"explanation\": \"Unparseable response\"}";
        
        // Very crude JSON parsing for demonstration, in a real app use a JSON serializer
        string sentiment = "Neutral";
        double score = 0.0;
        string explanation = "Detailed analysis results.";

        try {
            // Simulated parsing
            if (responseJson.Contains("\"sentiment\":")) sentiment = responseJson.Split("\"sentiment\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"score\":")) {
                var scorePart = responseJson.Split("\"score\":")[1].Split(",")[0].Split("}")[0].Trim();
                double.TryParse(scorePart, out score);
            }
            if (responseJson.Contains("\"explanation\":")) explanation = responseJson.Split("\"explanation\":")[1].Split("\"")[1];
        } catch { }

        // Persist
        var sessionId = SentimentId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "detailed-sentiment-model");
        var config = TextSentimentConfiguration.Of(LanguageCode.Of("en"));

        var session = TextSentimentSession.Create(sessionId, userId, modelId, config);

        var resultId = SentimentResultId.Of(Guid.NewGuid());
        var sentimentVo = SentimentText.Of(sentiment);
        var scoreVo = SentimentScore.Of(score);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = TextSentimentResult.Create(resultId, request.Text, sentimentVo, scoreVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeSentimentDetailedResult(sessionId.Value, resultId.Value, sentiment, score, explanation);
    }
}
