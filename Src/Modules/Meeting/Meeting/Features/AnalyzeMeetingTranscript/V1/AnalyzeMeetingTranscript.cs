using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public class AnalyzeMeetingTranscriptEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-transcript",
                async (AnalyzeMeetingTranscriptRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeMeetingTranscriptCommand(request.Transcript);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeMeetingTranscriptResponseDto(result.MeetingId, result.TranscriptId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeMeetingTranscript")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<AnalyzeMeetingTranscriptResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze Meeting Transcript")
            .WithDescription("Uses AI to summarize a meeting transcript and extract key insights.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
