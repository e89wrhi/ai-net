using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChatBot.Features.StreamResponse.V1;

public class StreamResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat/stream-response",
                (StreamAiResponseRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamAiResponseCommand(request.SessionId), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stream AI Response")
            .WithDescription("Streams the AI response for the given chat session.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
