using System.Security.Claims;
using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public class StreamAutoCompleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/stream",
                (StreamAutoCompleteRequestDto request, 
                 IMediator mediator,
                 IHttpContextAccessor httpContextAccessor, 
                 CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new StreamAutoCompleteCommand(userId, request.Prompt);
                    
                    return mediator.CreateStream(command, cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAutoComplete")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream AI Completion")
            .WithDescription("Streams text completion using an AI model.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
