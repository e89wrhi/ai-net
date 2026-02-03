using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.GetInvoices.V1;

public class GetInvoicesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/subscription/{{subscriptionId}}/invoices",
                async (Guid subscriptionId, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetInvoices(subscriptionId, userId), cancellationToken);

                    var response = result.Adapt<GetInvoicesResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetInvoices")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GetInvoicesResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Invoices")
            .WithDescription("Gets the invoices for a specific subscription, ensuring it belongs to the authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
