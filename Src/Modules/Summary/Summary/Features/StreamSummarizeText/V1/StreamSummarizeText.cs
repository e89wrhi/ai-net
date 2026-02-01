using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
using Summary.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Summary.Features.StreamSummarizeText.V1;

public record StreamSummarizeTextCommand(string Text, SummaryDetailLevel DetailLevel, string Language) : IStreamRequest<string>;

public class StreamSummarizeTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize-stream",
                (StreamSummarizeTextRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamSummarizeTextCommand(request.Text, request.DetailLevel, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamSummarizeText")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Text Summary")
            .WithDescription("Streams the generated summary of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamSummarizeTextRequestDto(string Text, SummaryDetailLevel DetailLevel, string Language);

internal class StreamSummarizeTextHandler : IStreamRequestHandler<StreamSummarizeTextCommand, string>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamSummarizeTextHandler(SummaryDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamSummarizeTextCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional summarization assistant."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var fullSummaryBuilder = new StringBuilder();
        int tokenEstimate = 0;

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullSummaryBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream completion
        await PersistSummaryAsync(request, fullSummaryBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistSummaryAsync(StreamSummarizeTextCommand request, string fullSummary, int tokenUsage, CancellationToken cancellationToken)
    {
        try 
        {
            var sessionId = SummaryId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "summary-stream-model");
            var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

            var session = TextSummarySession.Create(sessionId, userId, modelId, config);

            var resultId = SummaryResultId.Of(Guid.NewGuid());
            var summaryVo = SummaryText.Of(fullSummary);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var result = TextSummaryResult.Create(resultId, request.Text, summaryVo, tokenCountVo, costVo);
            session.AddResult(result);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log persistence error
        }
    }
}
