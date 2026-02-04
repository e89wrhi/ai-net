using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace LearningAssistant.Features.GenerateQuiz.V1;

public class GenerateQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/generate-quiz",
                async (GenerateQuizRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new GenerateQuizCommand(userId, request.Topic, request.QuestionCount, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateQuizResponseDto(result.SessionId, result.ActivityId, 
                        result.QuizContent,
                        result.ModelId, result.ProviderName));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateQuizResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Generate AI Quiz")
            .WithDescription("Uses AI to generate a quiz on a topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
