using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingTranscribedDomainEvent(MeetingId MeetingId, string Language) : IDomainEvent;