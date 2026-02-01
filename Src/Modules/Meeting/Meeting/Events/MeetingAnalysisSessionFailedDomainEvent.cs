using AI.Common.Core;
using Meeting.Enums;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingAnalysisSessionFailedDomainEvent(MeetingId MeetingId, MeetingAnalysisFailureReason Reason) : IDomainEvent;