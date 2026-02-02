using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoComplete.Features.StreamAutoComplete.V1;

internal class StreamAICompletionHandler : IStreamRequestHandler<StreamAutoCompleteCommand, string>
{
    private readonly IChatClient _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public StreamAICompletionHandler(IChatClient chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<string> Handle(StreamAutoCompleteCommand request,
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

    private async Task PersistInteractionAsync(StreamAutoCompleteCommand request, string fullResponse, int tokenCount, CancellationToken cancellationToken)
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
