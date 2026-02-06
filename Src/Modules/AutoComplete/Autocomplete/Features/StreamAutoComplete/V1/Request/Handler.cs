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

namespace AutoComplete.Features.StreamAutoComplete.V1;

internal class StreamAICompletionHandler : IStreamRequestHandler<StreamAutoCompleteCommand, string>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;
    private readonly AutocompleteDbContext _dbContext;

    public StreamAICompletionHandler(IAiOrchestrator orchestrator, IAiModelService modelService, AutocompleteDbContext dbContext)
    {
        _orchestrator = orchestrator;
        _modelService = modelService;
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<string> Handle(StreamAutoCompleteCommand request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var client = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        var fullResponseBuilder = new StringBuilder();
        long tokenCount = 0;

        await foreach (var update in client.GetStreamingResponseAsync(request.Prompt, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponseBuilder.Append(update.Text);
                yield return update.Text;
            }
        }

        // Crude estimation if usage wasn't provided
        tokenCount = (request.Prompt.Length + fullResponseBuilder.Length) / 4;

        // Persist after streaming finishes
        await PersistInteractionAsync(request, fullResponseBuilder.ToString(), (int)tokenCount, client, cancellationToken);
    }

    private async Task PersistInteractionAsync(StreamAutoCompleteCommand request, string fullResponse, int tokenCount, IChatClient client, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = AutoCompleteId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);
            
            // Use metadata from the client
            var clientMetadata = client.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "stream-model";
            var modelId = ModelId.Of(modelIdStr);

            var config = new AutoCompleteConfiguration(
                Temperature.Of(0.7f),
                TokenCount.Of(tokenCount),
                mode: request.Mode);

            var session = AutoCompleteSession.Create(sessionId, userId, modelId, config);

            var requestId = AutoCompleteRequestId.Of(Guid.NewGuid());
            var promptVo = AutoCompletePrompt.Of(request.Prompt);
            var suggestionVo = AutoCompleteSuggestion.Of(fullResponse);
            var tokenCountVo = TokenCount.Of(tokenCount);

            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenCount * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var autoCompleteRequest = AutoCompleteRequest.Create(
                requestId,
                promptVo,
                suggestionVo,
                tokenCountVo,
                costVo);

            session.AddRequest(autoCompleteRequest);

            _dbContext.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Logging would go here
        }
    }
}


