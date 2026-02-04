using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Resume.Features.AnalyzeResume.V1;

public class AnalyzeResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/analyze-ai",
                async (AnalyzeResumeRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeResumeCommand(request.ResumeContent, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeResumeResponseDto(result.SessionId, result.ResultId, 
                        result.Summary, result.Score,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<AnalyzeResumeResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze Resume with AI")
            .WithDescription("Uses AI to extract key information, skills, and experience from a resume.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
