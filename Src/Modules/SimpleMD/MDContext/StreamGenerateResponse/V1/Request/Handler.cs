using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using SimpleMD.Services;

namespace SimpleMD.Features.StreamGenerateResponse.V1;

internal class StreamGenerateResponseWithAIHandler : ICommandHandler<StreamGenerateResponseCommand, StreamGenerateResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IMarkdownFileProvider _markdownProvider;

    public StreamGenerateResponseWithAIHandler(IAiOrchestrator orchestrator, IMarkdownFileProvider markdownProvider)
    {
        _orchestrator = orchestrator;
        _markdownProvider = markdownProvider;
    }

    public async Task<StreamGenerateResponseCommandResult> Handle(StreamGenerateResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var markdown = await _markdownProvider.GetMarkdownAsync(cancellationToken);

        var messages = new[]
        {
            new ChatMessage(
                ChatRole.System,
                $"""
                You are a polite, professional assistant.

                RULES:
                - Start with a brief polite greeting (one sentence).
                - Answer clearly and concisely.
                - Do NOT mention the markdown document directly.
                - Use the markdown strictly as background context.

                ---
                {markdown}
                ---
                """),
            new ChatMessage(ChatRole.User, request.Text)
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var client = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        var metadata = client.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        // Returns the live streaming enumerable — consumed by the endpoint.
        var textStream = StreamChunksAsync(client, messages, cancellationToken);

        return new StreamGenerateResponseCommandResult(
            textStream,
            metadata?.DefaultModelId ?? "unknown",
            metadata?.ProviderName ?? "unknown");
    }

    private static async IAsyncEnumerable<string> StreamChunksAsync(
        IChatClient client,
        IEnumerable<ChatMessage> messages,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var update in client.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
                yield return update.Text;
        }
    }
}
