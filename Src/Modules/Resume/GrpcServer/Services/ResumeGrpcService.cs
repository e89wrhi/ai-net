using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;

using Protos = Resume.GrpcServer.Protos;
using Resume.GrpcServer.Protos;

namespace Resume.GrpcServer.Services;

public class ResumeGrpcService : Protos.ResumeGrpcService.ResumeGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ResumeGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<AnalyzeResumeResponse> AnalyzeResume(AnalyzeResumeRequest request, ServerCallContext context)
    {
        var cmd = new Resume.Features.AnalyzeResume.V1.AnalyzeResumeCommand(
            Guid.Parse(request.ResumeId),
            request.Summary,
            request.IncludeSkills,
            request.IncludeEducation,
            request.IncludeExperience,
            request.ParsedText);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeResumeResponse { Id = result.SessionId.ToString() };
    }
}
