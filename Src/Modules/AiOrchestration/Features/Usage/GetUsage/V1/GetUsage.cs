using AI.Common.Web;
using AiOrchestration.Features.Usage.GetUsage.V1.Request;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AiOrchestration.Features.Usage.GetUsage;

public class GetUsageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/orchestration/usage", async (DateTime? from, DateTime? to,
                IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetUsageQuery(from, to), cancellationToken);
            return Results.Ok(result.Usage);
        })
            .RequireAuthorization()
            .WithName("GetUsage")
            .WithSummary("Get AI usage statistics")
            .WithDescription("Retrieve historical data of AI requests and token consumption.")
            .WithOpenApi();

        return builder;
    }
}
