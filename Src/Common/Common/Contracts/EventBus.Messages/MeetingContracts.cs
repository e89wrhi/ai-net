using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record MeetingActionAdded(Guid Id) : IIntegrationEvent;
public record MeetingSummarized(Guid Id) : IIntegrationEvent;
public record MeetingTranscribed(Guid Id) : IIntegrationEvent;
public record MeetingUploaded(Guid Id) : IIntegrationEvent;