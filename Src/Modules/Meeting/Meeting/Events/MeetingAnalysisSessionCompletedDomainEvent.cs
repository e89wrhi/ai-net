using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingAnalysisSessionCompletedDomainEvent(MeetingId MeetingId) : IDomainEvent;