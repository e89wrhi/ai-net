using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChatBot.Features.StartChat.V1;

public class StartChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat", async (StartChatRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartChatCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartChatRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartChat")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<StartChatRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start Chat")
            .WithDescription("Start Chat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
