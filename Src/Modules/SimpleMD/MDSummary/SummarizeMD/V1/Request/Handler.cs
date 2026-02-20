using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using SimpleMD.Services;

namespace SimpleMD.Features.SummarizeMD.V1;

internal class SummarizeMDWithAIHandler : ICommandHandler<SummarizeMDCommand, SummarizeMDCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IMarkdownFileProvider _markdownProvider;

    public SummarizeMDWithAIHandler(IAiOrchestrator orchestrator, IMarkdownFileProvider markdownProvider)
    {
        _orchestrator = orchestrator;
        _markdownProvider = markdownProvider;
    }

    public async Task<SummarizeMDCommandResult> Handle(SummarizeMDCommand request, CancellationToken cancellationToken)
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

        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var summary = response.Messages.FirstOrDefault()?.Text ?? string.Empty;

        return new SummarizeMDCommandResult(
            summary,
            metadata?.DefaultModelId ?? "unknown",
            metadata?.ProviderName ?? "unknown");
    }
}
