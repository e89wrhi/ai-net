using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Summary.Features.StreamSummarizeText.V1;

public class StreamSummarizeTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize-stream",
                (StreamSummarizeTextRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new StreamSummarizeTextCommand(request.Text, request.DetailLevel, request.Language);
                    return Results.Ok(mediator.CreateStream(command, cancellationToken));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamSummarizeText")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream Text Summary")
            .WithDescription("Streams the generated summary of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
