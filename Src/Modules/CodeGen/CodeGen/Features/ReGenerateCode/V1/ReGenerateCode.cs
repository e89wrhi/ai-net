using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace CodeGen.Features.ReGenerateCode.V1;

public class ReGenerateCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/regenerate",
                async (ReGenerateCodeRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new ReGenerateCodeCommand(request.SessionId, request.Instruction);
                    var result = await mediator.Send(command, cancellationToken);

                    return Results.Ok(new ReGenerateCodeResponseDto(result.ResultId, result.Code));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReGenerateCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<ReGenerateCodeResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Re-generate Code")
            .WithDescription("Re-generates or refines code based on instructions and previous session state.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
