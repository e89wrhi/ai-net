using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SimplePlugin.Features.GenerateResponse.V1;

public class GenerateResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/plugin/functional",
                async (GenerateResponseRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var command = new GenerateResponseCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateResponseResponseDto( 
                        result.Response,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(Duende.IdentityServer.EntityFramework.Entities.ApiScope))
            .WithName("GenerateFunctionalResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("FunctionalCall").Build())
            .Produces<GenerateResponseResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Call AI Functions")
            .WithDescription("Uses AI to answer questions by calling local software tools (plugins).")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
