using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;
using Translate.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Translate.Features.DetectLanguage.V1;

public record DetectLanguageCommand(string Text) : ICommand<DetectLanguageResult>;

public record DetectLanguageResult(string DetectedLanguageCode, double Confidence);

public class DetectLanguageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/detect",
                async (DetectLanguageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new DetectLanguageCommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new DetectLanguageResponseDto(result.DetectedLanguageCode, result.Confidence));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DetectLanguage")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<DetectLanguageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Detect Language")
            .WithDescription("Uses AI to detect the language of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record DetectLanguageRequestDto(string Text);
public record DetectLanguageResponseDto(string DetectedLanguageCode, double Confidence);

internal class DetectLanguageHandler : ICommandHandler<DetectLanguageCommand, DetectLanguageResult>
{
    private readonly IChatClient _chatClient;

    public DetectLanguageHandler(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<DetectLanguageResult> Handle(DetectLanguageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var systemPrompt = "You are a language detection expert. Return ONLY the ISO 639-1 language code and a confidence score (0-1) separated by a comma. Example: en, 0.99";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Text)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Message.Text ?? "unknown, 0.0";
        
        var parts = responseText.Split(',');
        var langCode = parts[0].Trim();
        double confidence = 0.0;
        if (parts.Length > 1 && double.TryParse(parts[1].Trim(), out var parsedConfidence))
        {
            confidence = parsedConfidence;
        }

        return new DetectLanguageResult(langCode, confidence);
    }
}
