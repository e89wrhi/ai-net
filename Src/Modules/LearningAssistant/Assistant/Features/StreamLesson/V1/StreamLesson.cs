using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LearningAssistant.Features.StreamLesson.V1;

public class StreamLessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/lesson-stream",
                (StreamAILessonRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    return mediator.CreateStream(new StreamAILessonCommand(request.Topic, request.Level), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamLesson")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream AI Lesson")
            .WithDescription("Streams the AI generated lesson for the given topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
