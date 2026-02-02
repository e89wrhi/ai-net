using System.Runtime.CompilerServices;
using System.Text;
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

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public class StreamTranscribeAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/transcribe-stream",
                (StreamTranscribeAudioRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamTranscribeAudioCommand(request.AudioUrl, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamTranscribeAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Audio Transcription")
            .WithDescription("Streams the text transcription of an audio file in real-time.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
