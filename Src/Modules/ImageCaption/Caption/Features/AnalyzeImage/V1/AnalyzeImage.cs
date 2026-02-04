using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ImageCaption.Features.AnalyzeImage.V1;

public class AnalyzeImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/analyze",
                async (AnalyzeImageRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeImageCommand(userId, request.ImageUrlOrBase64, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeImageResponseDto(result.SessionId, result.ResultId, 
                        result.Analysis,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AnalyzeImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze Image with AI")
            .WithDescription("Uses AI to provide a detailed analysis of the provided image, including objects, colors, and context.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
