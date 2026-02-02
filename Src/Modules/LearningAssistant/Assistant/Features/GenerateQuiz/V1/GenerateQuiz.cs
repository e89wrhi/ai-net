using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public class GenerateQuizEndpoint : IMinimalEndpoint
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
            .WithName("GenerateQuiz")
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
