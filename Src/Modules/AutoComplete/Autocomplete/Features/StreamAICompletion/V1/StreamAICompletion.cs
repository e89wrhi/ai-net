using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.StreamAICompletion.V1;

public record StreamAICompletionCommand(Guid UserId, string Prompt) : IStreamRequest<string>;

public class StreamAICompletionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/stream",
                (StreamAICompletionRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    // Mock User ID or extract from context
                    var userId = Guid.NewGuid();
                    var command = new StreamAICompletionCommand(userId, request.Prompt);
                    
                    return mediator.CreateStream(command, cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAICompletion")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream AI Completion")
            .WithDescription("Streams text completion using an AI model.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamAICompletionRequestDto(string Prompt);

internal class StreamAICompletionHandler : IStreamRequestHandler<StreamAICompletionCommand, string>
{
    private readonly IChatClient _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public StreamAICompletionHandler(IChatClient chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<string> Handle(StreamAICompletionCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        var fullResponseBuilder = new StringBuilder();
        int tokenCount = 0;

        await foreach (var update in _chatClient.CompleteStreamingAsync(request.Prompt, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponseBuilder.Append(update.Text);
                tokenCount++; // Crude estimation, ideally usage comes from update or is calculated later
                yield return update.Text;
            }
        }

        // Persist after streaming finishes
        await PersistInteractionAsync(request, fullResponseBuilder.ToString(), tokenCount, cancellationToken);
    }

    private async Task PersistInteractionAsync(StreamAICompletionCommand request, string fullResponse, int tokenCount, CancellationToken cancellationToken)
    {
        try 
        {
            var sessionId = AutoCompleteId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "stream-model");
            var config = AutoCompleteConfiguration.Of("streaming");

            var session = AutoCompleteSession.Create(sessionId, userId, modelId, config);

            var requestId = AutoCompleteRequestId.Of(Guid.NewGuid());
            var promptVo = AutoCompletePrompt.Of(request.Prompt);
            var suggestionVo = AutoCompleteSuggestion.Of(fullResponse);
            var tokenCountVo = TokenCount.Of(tokenCount);
            
            // Estimate cost
            var costValue = (decimal)(tokenCount * 0.000002);
            var costVo = CostEstimate.Of(costValue);

            var autoCompleteRequest = AutoCompleteRequest.Create(
                requestId,
                promptVo,
                suggestionVo,
                tokenCountVo,
                costVo);

            session.AddRequest(autoCompleteRequest);

            // We need a new scope or use the existing DbContext if it's not disposed.
            // Since this is inside IAsyncEnumerable, the scope should be alive.
            _dbContext.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Logging would go here. 
            // We don't want to crash the stream if saving fails after streaming is done, 
            // although the client might have already disconnected.
        }
    }
}
