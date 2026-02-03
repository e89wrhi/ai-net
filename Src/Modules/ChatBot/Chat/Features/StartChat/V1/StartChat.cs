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

namespace ChatBot.Features.StartChat.V1;

public class StartChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat", async (StartChatRequest request,
                IMediator mediator, IHttpContextAccessor httpContextAccessor, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

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
