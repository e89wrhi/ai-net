using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Summary.Features.SummarizeText.V1;

public class SummarizeTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize",
                async (SummarizeTextWithAIRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new SummarizeTextWithAICommand(request.Text, request.DetailLevel, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new SummarizeTextWithAIResponseDto(result.SessionId, result.ResultId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SummarizeText")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<SummarizeTextWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Summarize Text")
            .WithDescription("Uses AI to generate a summary of the provided text based on the specified detail level.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
