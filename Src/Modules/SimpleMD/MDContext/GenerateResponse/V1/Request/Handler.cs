using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace SimpleMD.Features.GenerateResponse.V1;


internal class GenerateResponseWithAIHandler : ICommandHandler<GenerateResponseCommand, GenerateResponseCommandResult>
{
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateResponseWithAIHandler(IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<GenerateResponseCommandResult> Handle(GenerateResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var mdPath = Path.Combine(AppContext.BaseDirectory, "table.md");
        if (!File.Exists(mdPath))
            throw new FileNotFoundException("Context markdown file not found.", mdPath);

        var markdown = await File.ReadAllTextAsync(mdPath, cancellationToken);

        var messages = new[]
        {
            new ChatMessage(
                ChatRole.System,
                """
                You are a polite, professional assistant.

                RULES:
                - Start with a brief polite greeting (one sentence).
                - Answer clearly and concisely.
                - Do NOT mention the markdown.
                - Use the markdown strictly as background context.

                ---
                """ + markdown + """
                ---
                """
            ),
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