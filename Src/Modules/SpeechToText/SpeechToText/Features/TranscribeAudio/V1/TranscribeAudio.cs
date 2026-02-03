using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SpeechToText.Features.TranscribeAudio.V1;

public class TranscribeAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/transcribe",
                async (TranscribeAudioRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new TranscribeAudioCommand(request.AudioUrl, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new TranscribeAudioResponseDto(result.SessionId, result.ResultId, result.Transcript));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("TranscribeAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<TranscribeAudioResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Transcribe Audio to Text")
            .WithDescription("Uses AI to convert speech from an audio file into written text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
