using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Meeting.Features.ExtractActionItems.V1;

public class ExtractActionItemsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/action-items",
                async (ExtractActionItemsRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new ExtractActionItemsCommand(userId, request.Transcript, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ExtractActionItemsResponseDto(result.MeetingId, result.ActionItems,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ExtractActionItems")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<ExtractActionItemsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Extract Meeting Action Items")
            .WithDescription("Uses AI to extract only the actionable tasks and assignments from a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
