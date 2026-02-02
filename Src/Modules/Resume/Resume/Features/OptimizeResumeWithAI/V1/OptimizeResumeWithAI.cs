using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Resume.Data;
using Resume.Models;
using Resume.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;
using Resume.Enums;

namespace Resume.Features.OptimizeResumeWithAI.V1;

public class OptimizeResumeWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/optimize",
                async (OptimizeResumeWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new OptimizeResumeWithAICommand(request.ResumeContent, request.JobDescription);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new OptimizeResumeWithAIResponseDto(result.ResultId, result.OptimizedResume));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("OptimizeResumeWithAI")
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
