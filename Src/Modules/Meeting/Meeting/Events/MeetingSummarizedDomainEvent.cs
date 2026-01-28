using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingSummarizedDomainEvent(MeetingId MeetingId, string Transcript, string Summary) : IDomainEvent;