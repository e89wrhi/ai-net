using System.Runtime.CompilerServices;
using System.Text;
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

namespace LearningAssistant.Features.StreamAILesson.V1;

public class StreamAILessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/lesson-stream",
                (StreamAILessonRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamAILessonCommand(request.Topic, request.Level), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAILesson")
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
