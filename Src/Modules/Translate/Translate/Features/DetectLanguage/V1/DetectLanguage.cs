using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Translate.Features.DetectLanguage.V1;

public class DetectLanguageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/detect",
                async (DetectLanguageRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new DetectLanguageCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new DetectLanguageResponseDto(result.DetectedLanguageCode, 
                        result.Confidence,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DetectLanguage")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<DetectLanguageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Detect Language")
            .WithDescription("Uses AI to detect the language of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
