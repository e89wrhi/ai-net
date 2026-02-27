using AI.Common.Web;
using AiOrchestration.Features.ApiKeys.GetApiKeys.V1.Request;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AiOrchestration.Features.ApiKeys.GetApiKeys.V1;

public class GetApiKeysEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/orchestration/keys", async (IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetApiKeysQuery(), cancellationToken);
            return Results.Ok(result.Keys);
        })
            .RequireAuthorization()
            .WithName("GetApiKeys")
            .WithSummary("List all AI Provider API Keys")
            .WithDescription("Get a list of active API keys for the current user.")
            .WithOpenApi();

        return builder;
    }
}
