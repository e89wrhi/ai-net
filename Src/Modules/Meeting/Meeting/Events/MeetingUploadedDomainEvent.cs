using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingUploadedDomainEvent(MeetingId MeetingId, string OrganizerId, string Title) : IDomainEvent;