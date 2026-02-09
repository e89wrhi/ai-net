using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace SimpleMD.Features.SummarizeMD.V1;


internal class SummarizeMDWithAIHandler : ICommandHandler<SummarizeMDCommand, SummarizeMDCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public SummarizeMDWithAIHandler(IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<SummarizeMDCommandResult> Handle(SummarizeMDCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));
        var mdPath = Path.Combine(AppContext.BaseDirectory, "context.md");
        if (!File.Exists(mdPath))
            throw new FileNotFoundException("Markdown context file not found.", mdPath);

        var markdown = await File.ReadAllTextAsync(mdPath, cancellationToken);

        var messages = new List<ChatMessage>
        {
            new(
                ChatRole.System,
                """
                You are a technical documentation assistant.

                Summarize the following markdown content.
                Preserve meaning, structure, and key points.
                Do NOT invent information.

                Markdown:
                ---
                """ + markdown + """
                ---
                """
            ),
            new(
                ChatRole.User,
                request.Text // e.g. "short summary", "bullet points", "one paragraph"
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
