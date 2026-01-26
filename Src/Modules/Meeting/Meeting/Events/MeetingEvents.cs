using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingUploadedDomainEvent(MeetingId MeetingId, string Title) : IDomainEvent;
public record MeetingTranscribedDomainEvent(MeetingId MeetingId, string Language) : IDomainEvent;
public record MeetingSummarizedDomainEvent(MeetingId MeetingId, string Summary) : IDomainEvent;
public record ActionItemAddedDomainEvent(MeetingId MeetingId, string Description) : IDomainEvent;
