using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
