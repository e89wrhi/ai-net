using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public class StreamAnalyzeCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codedebug/analyze-stream",
                (StreamAnalyzeCodeRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    return mediator.CreateStream(new StreamAnalyzeCodeCommand(request.Code, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAnalyzeCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Analyze Code")
            .WithDescription("Streams the code analysis results using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
