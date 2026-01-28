using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
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
            MeetingTranscribedDomainEvent e => new MeetingTranscribed(e.MeetingId.Value),
            MeetingSummarizedDomainEvent e => new MeetingSummarized(e.MeetingId.Value),
            ActionItemAddedDomainEvent e => new MeetingActionAdded(e.MeetingId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            MeetingUploadedDomainEvent e => new UploadMeetingAudioMongo(e.MeetingId.Value, e.OrganizerId, e.Title, "Uploaded", DateTime.UtcNow),
            MeetingSummarizedDomainEvent e => new SummarizeMeetingAudioMongo(e.MeetingId.Value, e.Transcript, e.Summary, "Summarized"),
            _ => null
        };
    }
}