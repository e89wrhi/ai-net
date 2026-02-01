using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using AI.Contracts.EventBus.Messages;
using Meeting.Events;
using Meeting.Features.SummarizeMeetingAudio.V1;
using Meeting.Features.UploadMeetingAudio.V1;

namespace Meeting;

public sealed class MeetingEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            MeetingUploadedDomainEvent e => new MeetingUploaded(e.MeetingId.Value),
            MeetingAnalysisSessionFailedDomainEvent e => new MeetingTranscribed(e.MeetingId.Value),
            MeetingAnalysisSessionCompletedDomainEvent e => new MeetingSummarized(e.MeetingId.Value),
            MeetingAnalysisSessionStartedDomainEvent e => new MeetingActionAdded(e.MeetingId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            MeetingUploadedDomainEvent e => new UploadMeetingAudioMongo(e.MeetingId.Value, e.OrganizerId, e.Title, "Uploaded", DateTime.UtcNow),
            MeetingAnalysisSessionCompletedDomainEvent e => new SummarizeMeetingAudioMongo(e.MeetingId.Value, e.Transcript, e.Summary, "Summarized"),
            _ => null
        };
    }
}