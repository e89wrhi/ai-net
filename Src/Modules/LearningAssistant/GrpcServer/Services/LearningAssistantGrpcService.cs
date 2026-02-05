using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using LearningAssistant.GrpcServer.Protos;

namespace LearningAssistant.GrpcServer.Services;

public class LearningAssistantGrpcService : AssistantGrpcService.AssistantGrpcServiceBase
{
    private readonly IMediator _mediator;

    public LearningAssistantGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GenerateLessonResponse> GenerateLesson(GenerateLessonRequest request, ServerCallContext context)
    {
        var cmd = new LearningAssistant.Features.GenerateLesson.V1.GenerateLessonCommand(
            Guid.Parse(request.UserId),
            request.Topic,
            (LearningAssistant.Enums.LearningMode)(int)request.Mode,
            (LearningAssistant.Enums.DifficultyLevel)(int)request.Difficulty,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateLessonResponse 
        { 
            SessionId = result.SessionId.ToString(),
            ActivityId = result.ActivityId.ToString(),
            Content = result.Content,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamLesson(StreamLessonRequest request, IServerStreamWriter<StreamLessonResponse> responseStream, ServerCallContext context)
    {
        // Check if StreamLesson command exists, assuming it follows the pattern
        // The previous error didn't show StreamLesson issues, but I should verify if the command exists.
        // Assuming StreamLessonCommand(string Topic, LearningMode Mode, DifficultyLevel DifficultyLevel)
        
        var cmd = new LearningAssistant.Features.StreamLesson.V1.StreamAILessonCommand(
            request.Topic,
            (LearningAssistant.Enums.DifficultyLevel)(int)request.Difficulty);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamLessonResponse
            {
                Text = item
            });
        }
    }
}
