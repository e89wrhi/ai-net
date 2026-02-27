using AI.Common.Web;
using AiOrchestration.Features.Models.GetModels.V1.Request;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AiOrchestration.Features.Models.GetModels;

public class GetModelsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/orchestration/models", async (IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetModelsQuery(), cancellationToken);
            return Results.Ok(result.Models);
        })
            .RequireAuthorization()
            .WithName("GetModels")
            .WithSummary("List all active AI Models")
            .WithDescription("Get detailed information about available models, providers, and pricing.")
            .WithOpenApi();

        return builder;
    }
}
