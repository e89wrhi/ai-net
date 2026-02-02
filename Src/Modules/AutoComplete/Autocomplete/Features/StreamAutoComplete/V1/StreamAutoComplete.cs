using System.Security.Claims;
using AI.Common.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public class StreamAutoCompleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/stream",
                (StreamAutoCompleteRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        // For streaming endpoints returning IAsyncEnumerable, throwing or returning a special stream is an option.
                        // However, since it's RequireAuthorization, we can assume it's usually there or let it fail gracefully.
                        // Throwing will be caught by the global error handler.
                        throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
                    }

                    var command = new StreamAutoCompleteCommand(userId, request.Prompt);
                    
                    return mediator.CreateStream(command, cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAutoComplete")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream AI Completion")
            .WithDescription("Streams text completion using an AI model.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
