using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace SimplePlugin.Features.GenerateBingResponse.V1;

internal class GenerateBingResponseWithAIHandler : ICommandHandler<GenerateBingResponseCommand, GenerateBingResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;

    public GenerateBingResponseWithAIHandler(IAiOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<GenerateBingResponseCommandResult> Handle(GenerateBingResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a web search assistant. If you need to find real-time information, use the search_web tool."),
            new ChatMessage(ChatRole.User, request.Text)
        };

        // Define the search tool
        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(
                [Description("Searches the web for information using Bing.")]
                async ([Description("The search query.")] string query) => 
                {
                    // In a real scenario, this would call Bing API
                    await Task.Delay(100);
                    return $"Results for '{query}': [1] Bing is a web search engine owned and operated by Microsoft. [2] Search engines provide real-time data.";
                }, 
                "search_web")
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        using IChatClient toolCallingClient = chatClient.AsBuilder().UseFunctionInvocation().Build();

        var options = new ChatOptions { Tools = tools };

        var chatCompletion = await toolCallingClient.GetResponseAsync(messages, options, cancellationToken);
        var responseText = chatCompletion.Messages.FirstOrDefault()?.Text ?? string.Empty;

        return new GenerateBingResponseCommandResult(
            responseText, 
            clientMetadata?.DefaultModelId ?? "unknown", 
            clientMetadata?.ProviderName ?? "unknown");
    }
}
