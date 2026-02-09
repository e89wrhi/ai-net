using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace SimplePlugin.Features.GenerateBingResponse.V1;


internal class GenerateBingResponseWithAIHandler : ICommandHandler<GenerateBingResponseCommand, GenerateBingResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateBingResponseWithAIHandler(IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<GenerateBingResponseCommandResult> Handle(GenerateBingResponseCommand request, CancellationToken cancellationToken)
    {
        //kernel.ImportPluginFromObject(new WebSearchEnginePlugin(
        //    new BingConnector(Environment.GetEnvironmentVariable("BING_API_KEY"))));

        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a  analysis expert. Analyze the  of the following text. Return ONLY a single word (Positive, Negative, or Neutral) followed by a comma and a confidence score between 0 and 1. Example: Positive, 0.95"),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : request.Text)
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

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
        var responseText = chatCompletion.Messages[0].Text ?? "Neutral, 0.0";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Parse "Sentiment, Score"
        var parts = responseText.Split(',');
        var sentimentStr = parts[0].Trim();
        double score = 0.0;
        if (parts.Length > 1 && double.TryParse(parts[1].Trim(), out var parsedScore))
        {
            score = parsedScore;
        }

        return new GenerateBingResponseCommandResult(sentimentStr, modelIdStr, providerName);
    }
}
