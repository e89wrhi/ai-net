using LearningAssistant.Enums;
using MediatR;

namespace LearningAssistant.Features.StreamLesson.V1;

public record StreamAILessonCommand(Guid UserId, string Topic, LearningMode Mode, DifficultyLevel DifficultyLevel, string? ModelId = null) : IStreamRequest<string>;


