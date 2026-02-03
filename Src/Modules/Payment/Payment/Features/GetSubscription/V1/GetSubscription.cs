using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.GetSubscription.V1;

public class GetSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/subscription",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetSubscription(userId), cancellationToken);

                    var response = result.Adapt<GetSubscriptionResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GetSubscriptionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Subscription")
            .WithDescription("Gets the active subscription for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
