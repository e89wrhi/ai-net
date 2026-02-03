using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ImageCaption.Features.CaptionImage.V1;

public class CaptionImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/ai-caption",
                async (AIImageCaptionRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AIImageCaptionCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AIImageCaptionResponseDto(result.SessionId, result.ResultId, result.Caption));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CaptionImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AIImageCaptionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Image Caption with AI")
            .WithDescription("Uses AI to generate a descriptive caption for the provided image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
