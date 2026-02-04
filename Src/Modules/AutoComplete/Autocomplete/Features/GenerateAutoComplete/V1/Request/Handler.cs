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
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;
    private readonly AutocompleteDbContext _dbContext;

    public GenerateAICompletionHandler(IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient, AutocompleteDbContext dbContext)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<GenerateAutoCompleteCommandResult> Handle(GenerateAutoCompleteCommand request,
        CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System,
                    content: ""),
             new ChatMessage(
                    role: ChatRole.User,
                    content: request.Prompt)
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;
        
        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist Interaction
        var sessionId = AutoCompleteId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);

        // Configuration setup
        var config = new AutoCompleteConfiguration(
            Temperature.Of(0.7f),
            TokenCount.Of(tokenUsage),
            Enums.CompletionMode.Text
        );

        // Create Session Aggregate
        var session = AutoCompleteSession.Create(sessionId, userId, modelId, config);

        // Add Request to Session
        var requestId = AutoCompleteRequestId.Of(Guid.NewGuid());
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

