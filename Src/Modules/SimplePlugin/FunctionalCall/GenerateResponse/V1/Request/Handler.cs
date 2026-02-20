using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace SimplePlugin.Features.GenerateResponse.V1;

internal class GenerateResponseWithAIHandler : ICommandHandler<GenerateResponseCommand, GenerateResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;

    public GenerateResponseWithAIHandler(IAiOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<GenerateResponseCommandResult> Handle(GenerateResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant that can use tools to look up information about people, such as their age."),
            new ChatMessage(ChatRole.User, request.Text)
        };

        // Create tools from ContextInfo
        var contextInfo = new ContextInfo();
        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(contextInfo.GetAge, "GetAge")
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        var options = new ChatOptions
        {
            Tools = tools
        };

        // Call AI Model with tool calling enabled
        // Note: Real model would decide to call GetAge if needed.
        var chatCompletion = await chatClient.GetResponseAsync(messages, options, cancellationToken);
        var responseText = chatCompletion.Messages.FirstOrDefault()?.Text ?? string.Empty;

        return new GenerateResponseCommandResult(
            responseText, 
            clientMetadata?.DefaultModelId ?? "unknown", 
            clientMetadata?.ProviderName ?? "unknown");
    }
}
