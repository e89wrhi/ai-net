using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using CodeDebug;

namespace CodeDebug.GrpcServer.Services;

public class CodeDebugGrpcService : CodeDebug.CodeDebugGrpcService.CodeDebugGrpcServiceBase
{
    private readonly IMediator _mediator;

    public CodeDebugGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartCodeDebugResponse> StartCodeDebug(StartCodeDebugRequest request, ServerCallContext context)
    {
        var cmd = new CodeDebug.Features.StartCodeDebug.V1.StartCodeDebugCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartCodeDebugResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteCodeDebugResponse> DeleteCodeDebug(DeleteCodeDebugRequest request, ServerCallContext context)
    {
        var cmd = new CodeDebug.Features.DeleteCodeDebug.V1.DeleteCodeDebugCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteCodeDebugResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetCodeDebugHistoryResponse> GetCodeDebugHistory(GetCodeDebugHistoryRequest request, ServerCallContext context)
    {
        var query = new CodeDebug.Features.GetCodeDebugHistory.V1.GetCodeDebugHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetCodeDebugHistoryResponse();

        foreach (var dto in result.CodeDebugDtos)
        {
            var summary = new CodeDebugSummary
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

            // CodeDebugs are not included in CodeDebugDto currently; leave messages empty.
            response.CodeDebugs.Add(summary);
        }

        return response;
    }
}
