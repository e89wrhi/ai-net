using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.GenerateAutoComplete.V1;


internal class GenerateAICompletionHandler : ICommandHandler<GenerateAutoCompleteCommand, GenerateAutoCompleteCommandResult>
{
    private readonly IChatClient _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public GenerateAICompletionHandler(IChatClient chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<GenerateAutoCompleteCommandResult> Handle(GenerateAutoCompleteCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // 1. Call AI Model
        var chatCompletion = await _chatClient.CompleteAsync(request.Prompt, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Message.Text ?? string.Empty;

        // 2. Calculate Metadata
        // Use usage data from the provider if available, otherwise estimate
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (request.Prompt.Length + responseText.Length) / 4;

        // Simple cost estimation logic (example: $0.002 per 1k tokens)
        var costValue = (decimal)(tokenUsage * 0.000002);

        // 3. Persist Interaction
        var sessionId = AutoCompleteId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);

        // Use ModelId from metadata or fallback
        var modelIdStr = _chatClient.Metadata.ModelId ?? "default-model";
        var modelId = ModelId.Of(modelIdStr);

        var config = AutoCompleteConfiguration.Of("standard");

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

        return new GenerateAICompletionResult(responseText, tokenUsage, costValue);
    }
}
