using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public class StreamAutoCompleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/stream",
                (StreamAutoCompleteRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    // Mock User ID or extract from context
                    var userId = Guid.NewGuid();
                    var command = new StreamAutoCompleteCommand(userId, request.Prompt);
                    
                    return mediator.CreateStream(command, cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAutoComplete")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream AI Completion")
            .WithDescription("Streams text completion using an AI model.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
