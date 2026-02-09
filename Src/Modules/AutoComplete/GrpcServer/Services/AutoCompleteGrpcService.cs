using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using AutoComplete.GrpcServer.Protos;

using Protos = AutoComplete.GrpcServer.Protos;

namespace AutoComplete.GrpcServer.Services;

public class AutoCompleteGrpcService : Protos.AutoCompleteGrpcService.AutoCompleteGrpcServiceBase
{
    private readonly IMediator _mediator;

    public AutoCompleteGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GenerateAutoCompleteResponse> GenerateAutoComplete(GenerateAutoCompleteRequest request, ServerCallContext context)
    {
        var cmd = new AutoComplete.Features.GenerateAutoComplete.V1.GenerateAutoCompleteCommand(
            Guid.Parse(request.UserId),
            request.Prompt,
            (AutoComplete.Enums.CompletionMode)(int)request.Mode,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateAutoCompleteResponse
        {
            Completion = result.Completion,
            TokensUsed = result.TokensUsed,
            EstimatedCost = (double)result.EstimatedCost,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamAutoComplete(StreamAutoCompleteRequest request, IServerStreamWriter<StreamAutoCompleteResponse> responseStream, ServerCallContext context)
    {
        var cmd = new AutoComplete.Features.StreamAutoComplete.V1.StreamAutoCompleteCommand(
            Guid.Parse(request.UserId),
            request.Prompt,
            (AutoComplete.Enums.CompletionMode)(int)request.Mode,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamAutoCompleteResponse
            {
                Text = item
            });
        }
    }
}
