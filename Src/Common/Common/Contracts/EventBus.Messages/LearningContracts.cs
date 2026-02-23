using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record LearningProfileCreated(Guid Id) : IIntegrationEvent;
public record LessonGenerated(Guid Id) : IIntegrationEvent;
public record LearningProgressUpdated(Guid Id) : IIntegrationEvent;
public record QuizCompleted(Guid Id) : IIntegrationEvent;
public record LearningGoalAchieved(Guid Id, string GoalName) : IIntegrationEvent;
public record QuizFailed(Guid Id, int Score) : IIntegrationEvent;
public record ResourceDownloaded(Guid Id, string ResourceName) : IIntegrationEvent;
public record MentorAssigned(Guid Id, Guid MentorId) : IIntegrationEvent;