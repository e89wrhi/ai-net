using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record ActionItemAddedDomainEvent(MeetingId MeetingId, string Description) : IDomainEvent;
