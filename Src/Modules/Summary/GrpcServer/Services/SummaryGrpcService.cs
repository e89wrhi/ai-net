using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Summary.GrpcServer.Protos;

using Protos = Summary.GrpcServer.Protos;

namespace Summary.GrpcServer.Services;

public class SummaryGrpcService : Protos.SummaryGrpcService.SummaryGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SummaryGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<SummarizeTextResponse> SummarizeText(SummarizeTextRequest request, ServerCallContext context)
    {
        var cmd = new Summary.Features.SummarizeText.V1.SummarizeTextCommand(
            Guid.Parse(request.UserId),
            request.Text,
            (Summary.Enums.SummaryDetailLevel)(int)request.DetailLevel,
            request.Language,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new SummarizeTextResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Summary = result.Summary,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamSummarizeText(StreamSummarizeTextRequest request, IServerStreamWriter<StreamSummarizeTextResponse> responseStream, ServerCallContext context)
    {
        var cmd = new Summary.Features.StreamSummarizeText.V1.StreamSummarizeTextCommand(
            Guid.Parse(request.UserId),
            request.Text,
            (Summary.Enums.SummaryDetailLevel)(int)request.DetailLevel,
            request.Language,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamSummarizeTextResponse
            {
                Text = item
            });
        }
    }
}

