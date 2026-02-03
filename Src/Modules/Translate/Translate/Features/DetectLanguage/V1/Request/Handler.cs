using AI.Common.Core;
using AiOrchestration.Services;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;

namespace Translate.Features.DetectLanguage.V1;


internal class DetectLanguageHandler : ICommandHandler<DetectLanguageCommand, DetectLanguageCommandResult>
{
    private readonly IAiOrchestrator _chatClient;

    public DetectLanguageHandler(IAiOrchestrator chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<DetectLanguageCommandResult> Handle(DetectLanguageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var systemPrompt = "You are a language detection expert. Return ONLY the ISO 639-1 language code and a confidence score (0-1) separated by a comma. Example: en, 0.99";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Text)
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Messages[0].Text ?? "unknown, 0.0";

        var parts = responseText.Split(',');
        var langCode = parts[0].Trim();
        double confidence = 0.0;
        if (parts.Length > 1 && double.TryParse(parts[1].Trim(), out var parsedConfidence))
        {
            confidence = parsedConfidence;
        }

        return new DetectLanguageCommandResult(langCode, confidence);
    }
}
