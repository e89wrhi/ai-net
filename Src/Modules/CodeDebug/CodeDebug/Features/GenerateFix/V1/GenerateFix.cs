using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace CodeDebug.Features.GenerateFix.V1;

public class GenerateFixEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codedebug/fix",
                async (GenerateFixRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new GenerateFixCommand(request.SessionId, request.ReportId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateFixResponseDto(result.FixedCode, result.Explanation));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateFix")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<GenerateFixResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Generate Fix")
            .WithDescription("Generates fixed code based on a previous debugging analysis.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
