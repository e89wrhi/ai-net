using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record LearningProfileCreated(Guid Id) : IIntegrationEvent;
public record LessonGenerated(Guid Id) : IIntegrationEvent;
public record LearningProgressUpdated(Guid Id) : IIntegrationEvent;
public record QuizeCompleted(Guid Id) : IIntegrationEvent;