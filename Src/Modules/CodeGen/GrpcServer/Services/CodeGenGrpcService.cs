using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using CodeGen;

namespace CodeGen.GrpcServer.Services;

public class CodeGenGrpcService : CodeGen.CodeGenGrpcService.CodeGenGrpcServiceBase
{
    private readonly IMediator _mediator;

    public CodeGenGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartCodeGenResponse> StartCodeGen(StartCodeGenRequest request, ServerCallContext context)
    {
        var cmd = new CodeGen.Features.StartCodeGen.V1.StartCodeGenCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartCodeGenResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteCodeGenResponse> DeleteCodeGen(DeleteCodeGenRequest request, ServerCallContext context)
    {
        var cmd = new CodeGen.Features.DeleteCodeGen.V1.DeleteCodeGenCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteCodeGenResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetCodeGenHistoryResponse> GetCodeGenHistory(GetCodeGenHistoryRequest request, ServerCallContext context)
    {
        var query = new CodeGen.Features.GetCodeGenHistory.V1.GetCodeGenHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetCodeGenHistoryResponse();

        foreach (var dto in result.CodeGenDtos)
        {
            var summary = new CodeGenSummary
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

            // CodeGens are not included in CodeGenDto currently; leave messages empty.
            response.CodeGens.Add(summary);
        }

        return response;
    }
}
