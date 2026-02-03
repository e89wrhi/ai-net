using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.ForecastSpending.V1;

public class ForecastSpendingEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/payment/forecast",
                async (ForecastSpendingWithAIRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new ForecastSpendingWithAICommand(request.UserId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ForecastSpendingWithAIResponseDto(result.ForecastedAmount, result.Currency, result.Insights));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ForecastSpending")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<ForecastSpendingWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Forecast Spending with AI")
            .WithDescription("Uses AI to predict future AI costs based on historical usage charges.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
