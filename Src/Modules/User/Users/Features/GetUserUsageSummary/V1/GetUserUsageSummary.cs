using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace User.Features.GetUserUsageSummary.V1;

public class GetUserUsageSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/usage-summary",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetUserUsageSummary(userId), cancellationToken);

                    var response = result.Adapt<GetUserUsageSummaryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserUsageSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GetUserUsageSummaryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get User Usage Summary")
            .WithDescription("Gets the usage statistics for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

