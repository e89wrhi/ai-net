using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ChatBot.Features.UpdateChat.V1;

public class UpdateChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/chat/{{sessionId}}", async (Guid sessionId,
                UpdateChatRequest request, IMediator mediator, IHttpContextAccessor httpContextAccessor,
                IMapper mapper, CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateChatCommand(sessionId, userId, request.Title);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UpdateChatRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UpdateChat")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<UpdateChatRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Chat")
            .WithDescription("Updates chat session metadata like title")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
