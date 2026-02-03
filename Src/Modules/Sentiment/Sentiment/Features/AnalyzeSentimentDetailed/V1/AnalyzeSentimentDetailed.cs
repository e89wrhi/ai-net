using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;

public class AnalyzeSentimentDetailedEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/sentiment/analyze-detailed",
                async (AnalyzeSentimentDetailedRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeSentimentDetailedCommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeSentimentDetailedResponseDto(result.SessionId, result.ResultId, result.Sentiment, result.Score, result.Explanation));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeSentimentDetailed")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<AnalyzeSentimentDetailedResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Detailed Sentiment Analysis")
            .WithDescription("Uses AI to provide a deep sentiment analysis of the text, including an explanation of why the sentiment was chosen.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
