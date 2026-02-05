using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using TextToSpeech;

namespace TextToSpeech.GrpcServer.Services;

public class TextToSpeechGrpcService : TextToSpeechGrpcService.TextToSpeechGrpcServiceBase
{
    private readonly IMediator _mediator;

    public TextToSpeechGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartTextToSpeechResponse> StartTextToSpeech(StartTextToSpeechRequest request, ServerCallContext context)
    {
        var cmd = new TextToSpeech.Features.StartTextToSpeech.V1.StartTextToSpeechCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartTextToSpeechResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteTextToSpeechResponse> DeleteTextToSpeech(DeleteTextToSpeechRequest request, ServerCallContext context)
    {
        var cmd = new TextToSpeech.Features.DeleteTextToSpeech.V1.DeleteTextToSpeechCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteTextToSpeechResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetTextToSpeechHistoryResponse> GetTextToSpeechHistory(GetTextToSpeechHistoryRequest request, ServerCallContext context)
    {
        var query = new TextToSpeech.Features.GetTextToSpeechHistory.V1.GetTextToSpeechHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetTextToSpeechHistoryResponse();

        foreach (var dto in result.TextToSpeechDtos)
        {
            var summary = new TextToSpeechSummary
            {
                Id = dto.Id.ToString(),
                Title = dto.Title,
                Summary = dto.Summary,
                AiModelId = dto.AiModelId,
                SessionStatus = dto.SessionStatus,
                TotalTokens = dto.TotalTokens
            };

            // Map last sent timestamp if available
            if (dto.LastSentAt != default)
            {
                var utc = DateTime.SpecifyKind(dto.LastSentAt.ToUniversalTime(), DateTimeKind.Utc);
                summary.LastSentAt = Timestamp.FromDateTime(utc);
            }

            // TextToSpeechs are not included in TextToSpeechDto currently; leave messages empty.
            response.TextToSpeechs.Add(summary);
        }

        return response;
    }
}
