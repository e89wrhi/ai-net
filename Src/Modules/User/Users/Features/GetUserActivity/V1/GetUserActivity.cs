using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace User.Features.GetUserActivity.V1;

public class GetUserActivityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/activities",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetUserActivity(userId), cancellationToken);

                    var response = result.Adapt<GetUserActivityResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserActivity")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GetUserActivityResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Activity")
            .WithDescription("Gets the activity history for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

