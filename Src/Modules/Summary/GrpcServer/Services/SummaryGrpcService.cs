using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Summary;

namespace Summary.GrpcServer.Services;

public class SummaryGrpcService : Summary.SummaryGrpcService.SummaryGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SummaryGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartSummaryResponse> StartSummary(StartSummaryRequest request, ServerCallContext context)
    {
        var cmd = new Summary.Features.StartSummary.V1.StartSummaryCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartSummaryResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteSummaryResponse> DeleteSummary(DeleteSummaryRequest request, ServerCallContext context)
    {
        var cmd = new Summary.Features.DeleteSummary.V1.DeleteSummaryCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteSummaryResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetSummaryHistoryResponse> GetSummaryHistory(GetSummaryHistoryRequest request, ServerCallContext context)
    {
        var query = new Summary.Features.GetSummaryHistory.V1.GetSummaryHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetSummaryHistoryResponse();

        foreach (var dto in result.SummaryDtos)
        {
            var summary = new SummarySummary
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

            // Summarys are not included in SummaryDto currently; leave messages empty.
            response.Summarys.Add(summary);
        }

        return response;
    }
}
