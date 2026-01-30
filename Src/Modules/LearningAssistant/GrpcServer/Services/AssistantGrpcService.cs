using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;

namespace LearningAssistant.GrpcServer.Services;

public class AssistantGrpcService : LearningAssistant.AssistantGrpcService.AssistantGrpcServiceBase
{
    private readonly IMediator _mediator;

    public AssistantGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GenerateLessonResponse> GenerateLesson(GenerateLessonRequest request, ServerCallContext context)
    {
        var cmd = new LearningAssistant.Features.GenerateLesson.V1.GenerateLessonCommand(
            Guid.Parse(request.ProfileId),
            request.Title,
            request.Content,
            (LearningAssistant.Enums.DifficultyLevel)request.Level);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateLessonResponse { LessonId = result.Id.ToString() };
    }
}
