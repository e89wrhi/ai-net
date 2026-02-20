using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using SimpleMD.Services;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

internal class StreamSummarizeMDWithAIHandler : ICommandHandler<StreamSummarizeMDCommand, StreamSummarizeMDCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IMarkdownFileProvider _markdownProvider;

    public StreamSummarizeMDWithAIHandler(IAiOrchestrator orchestrator, IMarkdownFileProvider markdownProvider)
    {
        _orchestrator = orchestrator;
        _markdownProvider = markdownProvider;
    }

    public async Task<StreamSummarizeMDCommandResult> Handle(StreamSummarizeMDCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Instruction, nameof(request.Instruction));

        var markdown = await _markdownProvider.GetMarkdownAsync(cancellationToken);

        var messages = new List<ChatMessage>
        {
            new(
                ChatRole.System,
                $"""
                You are a technical documentation assistant.

                Summarize the following markdown document.
                Preserve meaning, structure, and key points.
                Do NOT invent information.

                Markdown:
                ---
                {markdown}
                ---
                """),
            new(
                ChatRole.User,
                request.Instruction // e.g. "short summary", "bullet points", "one paragraph"
            )
        };

        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        var metadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;

        var textStream = StreamChunksAsync(chatClient, messages, cancellationToken);

        return new StreamSummarizeMDCommandResult(
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
