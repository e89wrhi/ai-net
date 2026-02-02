using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Enums;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateAIQuiz.V1;

public class GenerateAIQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/generate-quiz",
                async (GenerateAIQuizRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAIQuizCommand(request.Topic, request.QuestionCount);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAIQuizResponseDto(result.SessionId, result.ActivityId, result.QuizContent));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAIQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateAIQuizResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Quiz")
            .WithDescription("Uses AI to generate a quiz on a topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
