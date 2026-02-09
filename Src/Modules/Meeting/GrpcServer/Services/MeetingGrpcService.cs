using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Meeting.Features.AnalyzeMeetingTranscript.V1;
using Meeting.Features.ExtractActionItems.V1;
using Meeting.Features.StreamMeetingAnalysis.V1;
using Meeting.Features.UploadMeetingAudio.V1;
using Meeting.GrpcServer.Protos;

using Protos = Meeting.GrpcServer.Protos;

namespace Meeting.GrpcServer.Services;

public class MeetingGrpcService : Protos.MeetingGrpcService.MeetingGrpcServiceBase
{
    private readonly IMediator _mediator;

    public MeetingGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<UploadMeetingAudioResponse> UploadMeetingAudio(Protos.UploadMeetingAudioRequest request, ServerCallContext context)
    {
        var cmd = new UploadMeetingAudioCommand(
            request.OrganizerId,
            request.Title,
            IncludeActionItems: true,
            IncludeDescisions: true,
            Language: "en",
            request.AudioUrl);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new UploadMeetingAudioResponse { Id = result.Id.ToString() };
    }

    public override async Task<AnalyzeMeetingTranscriptResponse> AnalyzeMeetingTranscript(AnalyzeMeetingTranscriptRequest request, ServerCallContext context)
    {
        var cmd = new AnalyzeMeetingTranscriptCommand(
            Guid.Parse(request.UserId),
            request.Transcript,
            request.IncludeActionItems,
            request.IncludeDescisions,
            request.Language,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeMeetingTranscriptResponse
        {
            MeetingId = result.MeetingId.ToString(),
            TranscriptId = result.TranscriptId.ToString(),
            Summary = result.Summary,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<ExtractActionItemsResponse> ExtractActionItems(ExtractActionItemsRequest request, ServerCallContext context)
    {
        var cmd = new ExtractActionItemsCommand(
            Guid.Parse(request.UserId),
            request.Transcript,
            request.IncludeActionItems,
            request.IncludeDescisions,
            request.Language,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ExtractActionItemsResponse
        {
            MeetingId = result.MeetingId.ToString(),
            ActionItems = result.ActionItems,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamMeetingAnalysis(StreamMeetingAnalysisRequest request, IServerStreamWriter<StreamMeetingAnalysisResponse> responseStream, ServerCallContext context)
    {
        var cmd = new StreamMeetingAnalysisCommand(
            Guid.Parse(request.UserId),
            request.Transcript,
            request.IncludeActionItems,
            request.IncludeDescisions,
            request.Language,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamMeetingAnalysisResponse
            {
                Text = item
            });
        }
    }
}
