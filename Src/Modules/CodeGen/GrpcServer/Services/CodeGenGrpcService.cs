using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using CodeGen.GrpcServer.Protos;

using Protos = CodeGen.GrpcServer.Protos;

namespace CodeGen.GrpcServer.Services;

public class CodeGenGrpcService : Protos.CodeGenGrpcService.CodeGenGrpcServiceBase
{
    private readonly IMediator _mediator;

    public CodeGenGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GenerateCodeResponse> GenerateCode(GenerateCodeRequest request, ServerCallContext context)
    {
        var cmd = new CodeGen.Features.GenerateCode.V1.GenerateCodeCommand(
            Guid.Parse(request.UserId),
            request.Prompt,
            request.Language,
            (CodeGen.Enums.CodeQualityLevel)(int)request.Quality,
            (CodeGen.Enums.CodeStyle)(int)request.Style,
            request.IncludeComments,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateCodeResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Code = result.Code,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<ReGenerateCodeResponse> ReGenerateCode(ReGenerateCodeRequest request, ServerCallContext context)
    {
        var cmd = new CodeGen.Features.ReGenerateCode.V1.ReGenerateCodeCommand(
            Guid.Parse(request.UserId),
            Guid.Parse(request.SessionId),
            request.Instruction,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ReGenerateCodeResponse
        {
            ResultId = result.ResultId.ToString(),
            Code = result.Code,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamGenerateCode(StreamGenerateCodeRequest request, IServerStreamWriter<StreamGenerateCodeResponse> responseStream, ServerCallContext context)
    {
        var cmd = new CodeGen.Features.StreamGenerateCode.V1.StreamGenerateCodeCommand(
            request.Prompt,
            request.Language);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamGenerateCodeResponse
            {
                Text = item
            });
        }
    }
}

