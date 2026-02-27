using AI.Common.Web;
using AiOrchestration.Features.ApiKeys.AddApiKey.V1.Request;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AiOrchestration.Features.ApiKeys.AddApiKey.V1;

public class AddApiKeyEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/orchestration/keys", async (AddApiKeyCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        })
            .RequireAuthorization()
            .WithName("AddApiKey")
            .WithSummary("Add a new AI Provider API Key")
            .WithDescription("Securely add an API key for a provider like OpenAI, Anthropic, or Ollama.")
            .WithOpenApi();

        return builder;
    }
}
