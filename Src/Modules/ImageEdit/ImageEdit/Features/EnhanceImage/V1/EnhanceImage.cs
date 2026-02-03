using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace ImageEdit.Features.EnhanceImage.V1;

public class EnhanceImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/enhance",
                async (AIEnhanceImageRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AIEnhanceImageCommand(request.ImageUrlOrBase64, request.Prompt);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AIEnhanceImageResponseDto(result.SessionId, result.ResultId, result.ResultImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("EnhanceImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<AIEnhanceImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Enhance Image with AI")
            .WithDescription("Uses AI to enhance the quality or details of an image based on a prompt.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
