using System.Runtime.CompilerServices;
using AI.Common.Core;
using AiOrchestration.Services;
using AiOrchestration.Models;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.GenerateAutoComplete.V1;


internal class GenerateAICompletionHandler : ICommandHandler<GenerateAutoCompleteCommand, GenerateAutoCompleteCommandResult>
{
    private readonly IAiOrchestrator _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public GenerateAICompletionHandler(IAiOrchestrator chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<GenerateAutoCompleteCommandResult> Handle(GenerateAutoCompleteCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        //  Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(request.Prompt, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata
        // Use usage data from the provider if available, otherwise estimate
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (request.Prompt.Length + responseText.Length) / 4;


        // Simple cost estimation logic (example: $0.002 per 1k tokens)
        var costValue = (decimal)(tokenUsage * 0.000002);

        // Persist Interaction
        var sessionId = AutoCompleteId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);

        // Use metadata from the client (following the example's GetService pattern)
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata ?? chatClient.Metadata;
        var modelIdStr = clientMetadata.ModelId ?? "default-model";
        var providerName = clientMetadata.ProviderName;
        
        var modelId = ModelId.Of(modelIdStr);


        var config = new AutoCompleteConfiguration()
        {
            MaxTokens = tokenUsage,
            Mode = Enums.CompletionMode.Text,
            Temperature = temperature,
        };

        // Create Session Aggregate
        var session = AutoCompleteSession.Create(sessionId, userId, modelId, config);

        // Add Request to Session
        var requestId = AutoCompleteRequestId.Of(Guid.NewGuid());

        // Assuming ValueObject.Of(string) pattern
        // If these VOs expect different types, this will need adjustment. 
        // Based on typical DDD patterns in this codebase (Of(primitive)).
        var promptVo = AutoCompletePrompt.Of(request.Prompt);
        var suggestionVo = AutoCompleteSuggestion.Of(responseText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var autoCompleteRequest = AutoCompleteRequest.Create(
            requestId,
            promptVo,
            suggestionVo,
            tokenCountVo,
            costVo);

        session.AddRequest(autoCompleteRequest);

        // Save to Database
        _dbContext.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAutoCompleteCommandResult(responseText, tokenUsage, costValue, modelIdStr, providerName);
    }
}

