using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace User.Features.AnalyzeUserUsage.V1;

public class AnalyzeUserUsageEndpoint : IMinimalEndpoint
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
            .WithName("AnalyzeUserUsage")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<AnalyzeUserUsageWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze User AI Usage")
            .WithDescription("Uses AI to analyze a user's AI consumption patterns and provide personalized recommendations.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
