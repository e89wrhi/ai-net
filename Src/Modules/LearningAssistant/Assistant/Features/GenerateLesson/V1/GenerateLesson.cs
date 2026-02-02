using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LearningAssistant.Features.GenerateLesson.V1;

public class GenerateLessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/generate-lesson",
                async (GenerateAILessonRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAILessonCommand(request.Topic, request.Level);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAILessonResponseDto(result.SessionId, result.ActivityId, result.Content));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateLesson")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateAILessonResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Lesson")
            .WithDescription("Uses AI to generate a lesson on a specific topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
