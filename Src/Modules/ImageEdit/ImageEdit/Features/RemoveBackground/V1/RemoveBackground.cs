using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ImageEdit.Features.RemoveBackground.V1;

public class RemoveBackgroundEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/remove-background",
                async (RemoveBackgroundRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new RemoveBackgroundCommand(userId, request.ImageUrlOrBase64, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new RemoveBackgroundResponseDto(result.SessionId, result.ResultId, 
                        result.ResultImageUrl,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("RemoveBackground")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<RemoveBackgroundResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Remove Background with AI")
            .WithDescription("Uses AI to automatically detect and remove the background from an image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
