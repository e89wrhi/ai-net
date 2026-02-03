using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.CreateSubscription.V1;


public class CreateSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/create", async (CreateSubscriptionRequest request,
                IMediator mediator, IHttpContextAccessor httpContextAccessor, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = mapper.Map<CreateSubscriptionCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<CreateSubscriptionRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CreateSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<CreateSubscriptionRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Subscription")
            .WithDescription("Create Subscription")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
