using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using SpeechToText;

namespace SpeechToText.GrpcServer.Services;

public class SpeechToTextGrpcService : SpeechToText.SpeechToTextGrpcService.SpeechToTextGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SpeechToTextGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartSpeechToTextResponse> StartSpeechToText(StartSpeechToTextRequest request, ServerCallContext context)
    {
        var cmd = new SpeechToText.Features.StartSpeechToText.V1.StartSpeechToTextCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartSpeechToTextResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteSpeechToTextResponse> DeleteSpeechToText(DeleteSpeechToTextRequest request, ServerCallContext context)
    {
        var cmd = new SpeechToText.Features.DeleteSpeechToText.V1.DeleteSpeechToTextCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteSpeechToTextResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetSpeechToTextHistoryResponse> GetSpeechToTextHistory(GetSpeechToTextHistoryRequest request, ServerCallContext context)
    {
        var query = new SpeechToText.Features.GetSpeechToTextHistory.V1.GetSpeechToTextHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetSpeechToTextHistoryResponse();

        foreach (var dto in result.SpeechToTextDtos)
        {
            var summary = new SpeechToTextSummary
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

            // SpeechToTexts are not included in SpeechToTextDto currently; leave messages empty.
            response.SpeechToTexts.Add(summary);
        }

        return response;
    }
}
