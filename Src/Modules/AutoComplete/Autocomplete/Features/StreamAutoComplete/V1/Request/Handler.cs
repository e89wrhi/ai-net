using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace AutoComplete.Features.StreamAutoComplete.V1;

internal class StreamAICompletionHandler : IStreamRequestHandler<StreamAutoCompleteCommand, string>
{
    private readonly IAiOrchestrator _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public StreamAICompletionHandler(IAiOrchestrator chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<string> Handle(StreamAutoCompleteCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use chatClient to get the best client for this request
        // Here we could pass specific criteria like: new ModelCriteria { MaxCostPerToken = 0.00001m }
        var client = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        var fullResponseBuilder = new StringBuilder();
        int tokenCount = 0;
        var response = await client.GetResponseAsync(request.Prompt, cancellationToken: cancellationToken);
        foreach (var update in response.Messages)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponseBuilder.Append(update.Text);
                tokenCount++; // Crude estimation, ideally usage comes from update or is calculated later
                yield return update.Text;
            }
        }

        // Persist after streaming finishes
        await PersistInteractionAsync(request, fullResponseBuilder.ToString(), tokenCount, client, cancellationToken);
    }

    private async Task PersistInteractionAsync(StreamAutoCompleteCommand request, string fullResponse, int tokenCount, IChatClient client, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = AutoCompleteId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);
            
            // Use metadata from the client (following the example's GetService pattern)
            var clientMetadata = client.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata ?? client.Metadata;
            var modelIdStr = clientMetadata?.ModelId ?? "stream-model";
            var modelId = ModelId.Of(modelIdStr);


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
