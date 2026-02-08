using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public class StreamGenerateBingResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/plugin/stream_bing",
                async (StreamGenerateBingResponseRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new StreamGenerateBingResponseCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new StreamGenerateBingResponseResponseDto( 
                        result.Response,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamGenerateBingResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<StreamGenerateBingResponseResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze Sentiment with AI")
            .WithDescription("Uses AI to analyze the sentiment of the provided text, returning sentiment type and confidence score.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
