using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Models;
using SpeechToText.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace SpeechToText.Features.TranscribeAudio.V1;

public record TranscribeAudioCommand(string AudioUrl, string Language) : ICommand<TranscribeAudioResult>;

public record TranscribeAudioResult(Guid SessionId, Guid ResultId, string Transcript);

public class TranscribeAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/transcribe",
                async (TranscribeAudioRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new TranscribeAudioCommand(request.AudioUrl, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new TranscribeAudioResponseDto(result.SessionId, result.ResultId, result.Transcript));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("TranscribeAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<TranscribeAudioResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Transcribe Audio to Text")
            .WithDescription("Uses AI to convert speech from an audio file into written text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record TranscribeAudioRequestDto(string AudioUrl, string Language);
public record TranscribeAudioResponseDto(Guid SessionId, Guid ResultId, string Transcript);

internal class TranscribeAudioHandler : ICommandHandler<TranscribeAudioCommand, TranscribeAudioResult>
{
    private readonly SpeechToTextDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public TranscribeAudioHandler(SpeechToTextDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<TranscribeAudioResult> Handle(TranscribeAudioCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.AudioUrl, nameof(request.AudioUrl));

        // Multi-modal models can process audio if supported, 
        // otherwise this serves as an orchestration point.
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a transcription engine."),
            new ChatMessage(ChatRole.User, $"Please transcribe the audio at this URL: {request.AudioUrl} in language: {request.Language}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var transcriptText = completion.Message.Text ?? "Transcription failed.";

        // Persist
        var sessionId = SpeechToTextId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); 
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "whisper-v3");
        var config = SpeechToTextConfiguration.Of(LanguageCode.Of(request.Language));

        var session = SpeechToTextSession.Create(sessionId, userId, modelId, config);

        var resultId = SpeechToTextResultId.Of(Guid.NewGuid());
        var audioVo = AudioSource.Of(request.AudioUrl);
        var transcriptVo = Transcript.Of(transcriptText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = SpeechToTextResult.Create(resultId, audioVo, transcriptVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TranscribeAudioResult(sessionId.Value, resultId.Value, transcriptText);
    }
}
