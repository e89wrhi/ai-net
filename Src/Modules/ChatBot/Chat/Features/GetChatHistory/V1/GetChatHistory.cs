using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ChatBot.Features.GetChatHistory.V1;

public class GetChatHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/chat/history",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetChatHistory(userId), cancellationToken);

                    var response = result.Adapt<GetChatHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetChatHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<GetChatHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Chat History")
            .WithDescription("Gets the chat session history for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
