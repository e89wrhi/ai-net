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

namespace ChatBot.Features.SendMessage.V1;

public class SendMessageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat/send-message", async (SendMessageRequest request,
                IMediator mediator, IHttpContextAccessor httpContextAccessor, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = mapper.Map<SendMessageCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendMessageRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendMessage")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<SendMessageRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send Message")
            .WithDescription("Send Message")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
