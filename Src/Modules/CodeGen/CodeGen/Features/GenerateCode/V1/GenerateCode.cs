using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace CodeGen.Features.GenerateCode.V1;

public class GenerateCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/generate",
                async (GenerateCodeRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new GenerateCodeCommand(request.Prompt, request.Language);
                    var result = await mediator.Send(command, cancellationToken);

                    return Results.Ok(new GenerateCodeResponseDto(result.SessionId, result.ResultId, result.Code));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<GenerateCodeResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Generate Code")
            .WithDescription("Generates code based on a text prompt using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
