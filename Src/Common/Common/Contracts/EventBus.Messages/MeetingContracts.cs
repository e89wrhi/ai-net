using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record MeetingActionAdded(Guid Id) : IIntegrationEvent;
public record MeetingSummarized(Guid Id) : IIntegrationEvent;
public record MeetingTranscribed(Guid Id) : IIntegrationEvent;
public record MeetingUploaded(Guid Id) : IIntegrationEvent;
public record MeetingScheduled(Guid Id, DateTime StartTime) : IIntegrationEvent;
public record MeetingStarted(Guid Id) : IIntegrationEvent;
public record MeetingEnded(Guid Id) : IIntegrationEvent;
public record ParticipantJoined(Guid Id, Guid UserId) : IIntegrationEvent;
public record ParticipantLeft(Guid Id, Guid UserId) : IIntegrationEvent;