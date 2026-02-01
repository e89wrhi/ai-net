using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Translate;

namespace Translate.GrpcServer.Services;

public class TranslateGrpcService : Translate.TranslateGrpcService.TranslateGrpcServiceBase
{
    private readonly IMediator _mediator;

    public TranslateGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartTranslateResponse> StartTranslate(StartTranslateRequest request, ServerCallContext context)
    {
        var cmd = new Translate.Features.StartTranslate.V1.StartTranslateCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartTranslateResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteTranslateResponse> DeleteTranslate(DeleteTranslateRequest request, ServerCallContext context)
    {
        var cmd = new Translate.Features.DeleteTranslate.V1.DeleteTranslateCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteTranslateResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetTranslateHistoryResponse> GetTranslateHistory(GetTranslateHistoryRequest request, ServerCallContext context)
    {
        var query = new Translate.Features.GetTranslateHistory.V1.GetTranslateHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetTranslateHistoryResponse();

        foreach (var dto in result.TranslateDtos)
        {
            var summary = new TranslateSummary
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

            // Translates are not included in TranslateDto currently; leave messages empty.
            response.Translates.Add(summary);
        }

        return response;
    }
}
