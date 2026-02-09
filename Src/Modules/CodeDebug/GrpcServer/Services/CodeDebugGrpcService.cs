using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using CodeDebug.GrpcServer.Protos;

using Protos = CodeDebug.GrpcServer.Protos;

namespace CodeDebug.GrpcServer.Services;

public class CodeDebugGrpcService : Protos.CodeDebugGrpcService.CodeDebugGrpcServiceBase
{
    private readonly IMediator _mediator;

    public CodeDebugGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<AnalyzeCodeResponse> AnalyzeCode(AnalyzeCodeRequest request, ServerCallContext context)
    {
        var cmd = new CodeDebug.Features.AnalyzeCode.V1.AnalyzeCodeCommand(
            Guid.Parse(request.UserId),
            request.Code,
            (CodeDebug.Enums.ProgrammingLanguage)(int)request.Language,
            (CodeDebug.Enums.DebugDepth)(int)request.Depth,
            request.IncludeSuggestion,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeCodeResponse
        {
            SessionId = result.SessionId.ToString(),
            ReportId = result.ReportId.ToString(),
            Summary = result.Summary,
            IssueCount = result.IssueCount,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamAnalyzeCode(StreamAnalyzeCodeRequest request, IServerStreamWriter<StreamAnalyzeCodeResponse> responseStream, ServerCallContext context)
    {
        var cmd = new CodeDebug.Features.StreamAnalyzeCode.V1.StreamAnalyzeCodeCommand(
            Guid.Parse(request.UserId),
            request.Code,
            (CodeDebug.Enums.ProgrammingLanguage)(int)request.Language,
            (CodeDebug.Enums.DebugDepth)(int)request.Depth,
            request.IncludeSuggestion,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamAnalyzeCodeResponse
            {
                Text = item
            });
        }
    }

    public override async Task<GenerateFixResponse> GenerateFix(GenerateFixRequest request, ServerCallContext context)
    {
        var cmd = new CodeDebug.Features.GenerateFix.V1.GenerateFixCommand(
            Guid.Parse(request.UserId),
            Guid.Parse(request.SessionId),
            Guid.Parse(request.ReportId),
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateFixResponse
        {
            FixedCode = result.FixedCode,
            Explanation = result.Explanation,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

