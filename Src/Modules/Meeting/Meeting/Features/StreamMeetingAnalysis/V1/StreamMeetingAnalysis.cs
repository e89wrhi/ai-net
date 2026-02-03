using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public class StreamMeetingAnalysisEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-stream",
                (StreamMeetingAnalysisRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new StreamMeetingAnalysisCommand(request.Transcript);
                    return Results.Ok(mediator.CreateStream(command, cancellationToken));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamMeetingAnalysis")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream Meeting Analysis")
            .WithDescription("Streams the AI analysis of a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
