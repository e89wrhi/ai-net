using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace SimplePlugin.Features.StreamGenerateResponse.V1;

internal class StreamGenerateResponseWithAIHandler : ICommandHandler<StreamGenerateResponseCommand, StreamGenerateResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;

    public StreamGenerateResponseWithAIHandler(IAiOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<StreamGenerateResponseCommandResult> Handle(StreamGenerateResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant that can use tools to look up information about people."),
            new ChatMessage(ChatRole.User, request.Text)
        };

        // Create tools
        var contextInfo = new ContextInfo();
        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(contextInfo.GetAge, "GetAge")
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        // Wrap the client to handle tool calls automatically
        using IChatClient toolCallingClient = chatClient.AsBuilder().UseFunctionInvocation().Build();

        var options = new ChatOptions { Tools = tools };

        // Start streaming
        var textStream = StreamChunksAsync(toolCallingClient, messages, options, cancellationToken);

        return new StreamGenerateResponseCommandResult(
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
