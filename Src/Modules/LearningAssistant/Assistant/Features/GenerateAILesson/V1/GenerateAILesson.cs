using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using LearningAssistant.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateAILesson.V1;

public class GenerateAILessonEndpoint : IMinimalEndpoint
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
            .WithName("GenerateAILesson")
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
