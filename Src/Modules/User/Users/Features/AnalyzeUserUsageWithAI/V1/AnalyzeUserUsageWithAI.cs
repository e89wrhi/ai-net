using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using User.Data;
using User.Models;
using User.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.Entities;

namespace User.Features.AnalyzeUserUsageWithAI.V1;

public class AnalyzeUserUsageWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/user/analyze-usage",
                async (AnalyzeUserUsageWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeUserUsageWithAICommand(request.UserId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeUserUsageWithAIResponseDto(result.UsageSummary, result.Recommendations));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeUserUsageWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<AnalyzeUserUsageWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze User AI Usage")
            .WithDescription("Uses AI to analyze a user's AI consumption patterns and provide personalized recommendations.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
