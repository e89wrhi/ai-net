using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

internal class StreamGenerateBingResponseWithAIHandler : ICommandHandler<StreamGenerateBingResponseCommand, StreamGenerateBingResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;

    public StreamGenerateBingResponseWithAIHandler(IAiOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<StreamGenerateBingResponseCommandResult> Handle(StreamGenerateBingResponseCommand request, CancellationToken cancellationToken)
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
                    await Task.Delay(100);
                    return $"Found results for '{query}': Search results are usually displayed here.";
                }, 
                "search_web")
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        using IChatClient toolCallingClient = chatClient.AsBuilder().UseFunctionInvocation().Build();

        var options = new ChatOptions { Tools = tools };

        // Start streaming
        var textStream = StreamChunksAsync(toolCallingClient, messages, options, cancellationToken);

        return new StreamGenerateBingResponseCommandResult(
            textStream,
            clientMetadata?.DefaultModelId ?? "unknown",
            clientMetadata?.ProviderName ?? "unknown");
    }

    private static async IAsyncEnumerable<string> StreamChunksAsync(
        IChatClient client,
        IEnumerable<ChatMessage> messages,
        ChatOptions options,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var update in client.GetStreamingResponseAsync(messages, options, cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
                yield return update.Text;
        }
    }
}
