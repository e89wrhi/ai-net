using AI.Common.Web;
using AiOrchestration.Features.ApiKeys.RemoveApiKey.V1.Request;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AiOrchestration.Features.ApiKeys.RemoveApiKey.V1;

public class RemoveApiKeyEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/orchestration/keys/{{id:guid}}", async (Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            await mediator.Send(new RemoveApiKeyCommand(id), cancellationToken);
            return Results.NoContent();
        })
            .RequireAuthorization()
            .WithName("RemoveApiKey")
            .WithSummary("Remove an AI Provider API Key")
            .WithDescription("Mark an API key as inactive/removed.")
            .WithOpenApi();

        return builder;
    }
}
