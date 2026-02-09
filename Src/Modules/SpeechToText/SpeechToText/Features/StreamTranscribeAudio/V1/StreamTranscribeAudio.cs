using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public class StreamTranscribeAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/transcribe-stream",
                (StreamTranscribeAudioRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new StreamTranscribeAudioCommand(userId, request.AudioUrl, request.Language,
                        request.IncludePunctuation, request.DetailLevel, request.ModelId);


                    return Results.Ok(mediator.CreateStream(command, cancellationToken));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamTranscribeAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream Audio Transcription")
            .WithDescription("Streams the text transcription of an audio file in real-time.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
