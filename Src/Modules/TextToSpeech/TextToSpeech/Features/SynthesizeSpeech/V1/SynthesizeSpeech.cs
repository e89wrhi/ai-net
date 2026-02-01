using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;
using TextToSpeech.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public record SynthesizeSpeechCommand(string Text, VoiceType Voice, SpeechSpeed Speed, string Language) : ICommand<SynthesizeSpeechResult>;

public record SynthesizeSpeechResult(Guid SessionId, Guid ResultId, string AudioUrl);

public class SynthesizeSpeechEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/synthesize",
                async (SynthesizeSpeechRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new SynthesizeSpeechCommand(request.Text, request.Voice, request.Speed, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new SynthesizeSpeechResponseDto(result.SessionId, result.ResultId, result.AudioUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SynthesizeSpeech")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<SynthesizeSpeechResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Synthesize Text to Speech")
            .WithDescription("Uses AI to convert written text into high-quality spoken audio.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record SynthesizeSpeechRequestDto(string Text, VoiceType Voice, SpeechSpeed Speed, string Language);
public record SynthesizeSpeechResponseDto(Guid SessionId, Guid ResultId, string AudioUrl);

internal class SynthesizeSpeechHandler : ICommandHandler<SynthesizeSpeechCommand, SynthesizeSpeechResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public SynthesizeSpeechHandler(TextToSpeechDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<SynthesizeSpeechResult> Handle(SynthesizeSpeechCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        // Use IChatClient to orchestrate or log the synthesis intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a speech synthesis orchestrator."),
            new ChatMessage(ChatRole.User, $"Synthesize this text using {request.Voice} voice at {request.Speed} speed in {request.Language}: {request.Text}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        
        // Mock Audio URL (In real implementation, this would call a TTS service like OpenAI TTS or Azure Speech)
        var audioUrl = $"https://ai-audio-storage.com/{Guid.NewGuid()}.mp3";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "tts-1-hd");
        var config = new TextToSpeechConfiguration(request.Voice, request.Speed, LanguageCode.Of(request.Language));

        var session = TextToSpeechSession.Create(sessionId, userId, modelId, config);

        var resultId = TextToSpeechResultId.Of(Guid.NewGuid());
        var speechVo = SynthesizedSpeech.Of(audioUrl);
        var tokenCountVo = TokenCount.Of(request.Text.Length); // For TTS, tokens might be char count or similar
        var costVo = CostEstimate.Of(0.015m);

        var result = TextToSpeechResult.Create(resultId, request.Text, speechVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SynthesizeSpeechResult(sessionId.Value, resultId.Value, audioUrl);
    }
}
