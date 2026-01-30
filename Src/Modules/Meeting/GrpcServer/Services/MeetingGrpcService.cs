using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;

namespace Meeting.GrpcServer.Services;

public class MeetingGrpcService : Meeting.MeetingGrpcService.MeetingGrpcServiceBase
{
    private readonly IMediator _mediator;

    public MeetingGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<UploadMeetingAudioResponse> UploadMeetingAudio(UploadMeetingAudioRequest request, ServerCallContext context)
    {
        var cmd = new Meeting.Features.UploadMeetingAudio.V1.UploadMeetingAudioCommand(
            request.OrganizerId,
            request.Title,
            request.AudioUrl);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new UploadMeetingAudioResponse { Id = result.Id.ToString() };
    }

    public override async Task<TranscribeMeetingResponse> TranscribeMeeting(TranscribeMeetingRequest request, ServerCallContext context)
    {
        var cmd = new Meeting.Features.SummarizeMeetingAudio.V1.SummarizeMeetingAudioCommand(
            Guid.Parse(request.MeetingId),
            request.TranscriptText,
            request.Language,
            request.Confidence,
            request.Summary);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new TranscribeMeetingResponse { Id = result.Id.ToString() };
    }

    public override async Task<GetMeetingSummaryResponse> GetMeetingSummary(GetMeetingSummaryRequest request, ServerCallContext context)
    {
        var query = new Meeting.Features.GetMeetingSummary.V1.GetMeetingSummary(Guid.Parse(request.MeetingId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var dto = result.MeetingSummaryDto;

        var response = new GetMeetingSummaryResponse
        {
            Summary = new MeetingSummary
            {
                Id = dto.Id.ToString(),
                Title = dto.Title ?? string.Empty,
                Summary = dto.Summary ?? string.Empty,
                Status = dto.Status ?? string.Empty
            }
        };

        if (dto.CreatedAt != default)
        {
            var utc = DateTime.SpecifyKind(dto.CreatedAt.ToUniversalTime(), DateTimeKind.Utc);
            response.Summary.CreatedAt = Timestamp.FromDateTime(utc);
        }

        return response;
    }
}
