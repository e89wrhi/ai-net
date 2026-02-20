using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SimpleMD.Features.GenerateResponse.V1;

public class GenerateResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/md/chat",
                async (GenerateResponseRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var command = new GenerateResponseCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateResponseResponseDto(result.Response, result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateMDContextResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("SimpleMD").Build())
            .Produces<GenerateResponseResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Generate MD Context Response")
            .WithDescription("Uses AI to answer a question, with a markdown file injected as background context.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
