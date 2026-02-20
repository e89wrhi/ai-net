using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using SimpleMD.Services;

namespace SimpleMD.Features.GenerateResponse.V1;

internal class GenerateResponseWithAIHandler : ICommandHandler<GenerateResponseCommand, GenerateResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IMarkdownFileProvider _markdownProvider;

    public GenerateResponseWithAIHandler(IAiOrchestrator orchestrator, IMarkdownFileProvider markdownProvider)
    {
        _orchestrator = orchestrator;
        _markdownProvider = markdownProvider;
    }

    public async Task<GenerateResponseCommandResult> Handle(GenerateResponseCommand request, CancellationToken cancellationToken)
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

        var response = await client.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = response.Messages.FirstOrDefault()?.Text ?? string.Empty;

        return new GenerateResponseCommandResult(
            responseText,
            metadata?.DefaultModelId ?? "unknown",
            metadata?.ProviderName ?? "unknown");
    }
}