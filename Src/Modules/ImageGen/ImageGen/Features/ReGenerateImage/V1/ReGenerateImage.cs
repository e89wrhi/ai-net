using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ImageGen.Features.ReGenerateImage.V1;

public class ReGenerateImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/regenerate",
                async (ReGenerateImageRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new ReGenerateImageCommand(userId, request.SessionId, request.Instruction, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ReGenerateImageResponseDto(result.ResultId, result.ImageUrl,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReGenerateImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<ReGenerateImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Re-generate Image")
            .WithDescription("Re-generates or modifies an existing image based on new instructions.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
