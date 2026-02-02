using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Resume.Features.AnalyzeResume.V1;

public class AnalyzeResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/analyze-ai",
                async (AnalyzeResumeWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeResumeWithAICommand(request.ResumeContent);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeResumeWithAIResponseDto(result.SessionId, result.ResultId, result.Summary, result.Score));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<AnalyzeResumeWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Resume with AI")
            .WithDescription("Uses AI to extract key information, skills, and experience from a resume.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
