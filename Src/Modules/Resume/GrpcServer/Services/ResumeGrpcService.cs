using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;

namespace Resume.GrpcServer.Services;

public class ResumeGrpcService : Resume.ResumeGrpcService.ResumeGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ResumeGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<UploadResumeResponse> UploadResume(UploadResumeRequest request, ServerCallContext context)
    {
        var cmd = new Resume.Features.UploadResume.V1.UploadResumeCommand(
            request.UserId,
            request.CandidateName,
            request.ResumeUrl,
            request.FileName);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new UploadResumeResponse { Id = result.Id.ToString() };
    }

    public override async Task<AnalyzeResumeResponse> AnalyzeResume(AnalyzeResumeRequest request, ServerCallContext context)
    {
        var cmd = new Resume.Features.AnalyzeResume.V1.AnalyzeResumeCommand(
            Guid.Parse(request.ResumeId),
            request.Summary,
            request.ParsedText);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeResumeResponse { Id = result.Id.ToString() };
    }

    public override async Task<ReAnalyzeResumeResponse> ReAnalyzeResume(ReAnalyzeResumeRequest request, ServerCallContext context)
    {
        var cmd = new Resume.Features.ReAnalyzeResume.V1.ReAnalyzeResumeCommand(
            Guid.Parse(request.ResumeId),
            request.Summary,
            request.ParsedText);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ReAnalyzeResumeResponse { Id = result.Id.ToString() };
    }

    public override async Task<GetResumeAnalysisResponse> GetResumeAnalysis(GetResumeAnalysisRequest request, ServerCallContext context)
    {
        var query = new Resume.Features.GetResumeAnalysis.V1.GetResumeAnalysis(Guid.Parse(request.ResumeId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var dto = result.ResumeAnalysisDto;

        var response = new GetResumeAnalysisResponse
        {
            Analysis = new ResumeAnalysis
            {
                Id = dto.Id.ToString(),
                CandidateName = dto.CandidateName ?? string.Empty,
                Summary = dto.Summary ?? string.Empty,
                Status = dto.Status ?? string.Empty
            }
        };

        if (dto.AnalyzedAt.HasValue)
        {
            var utc = DateTime.SpecifyKind(dto.AnalyzedAt.Value.ToUniversalTime(), DateTimeKind.Utc);
            response.Analysis.AnalyzedAt = Timestamp.FromDateTime(utc);
        }

        return response;
    }
}
