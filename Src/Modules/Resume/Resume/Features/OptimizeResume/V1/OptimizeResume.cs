using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Resume.Features.OptimizeResume.V1;

public class OptimizeResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/optimize",
                async (OptimizeResumeWithAIRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new OptimizeResumeWithAICommand(request.ResumeContent, request.JobDescription);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new OptimizeResumeWithAIResponseDto(result.ResultId, result.OptimizedResume));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("OptimizeResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<OptimizeResumeWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Optimize Resume for Job Description")
            .WithDescription("Uses AI to suggest improvements and tailor the resume for a specific job description.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
