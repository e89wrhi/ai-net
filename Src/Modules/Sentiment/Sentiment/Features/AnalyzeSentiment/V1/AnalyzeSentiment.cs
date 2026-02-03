using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Sentiment.Features.AnalyzeSentiment.V1;

public class AnalyzeSentimentEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/sentiment/analyze",
                async (AnalyzeSentimentWithAIRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeSentimentWithAICommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeSentimentWithAIResponseDto(result.SessionId, result.ResultId, result.Sentiment, result.Score));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeSentiment")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<AnalyzeSentimentWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Sentiment with AI")
            .WithDescription("Uses AI to analyze the sentiment of the provided text, returning sentiment type and confidence score.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
