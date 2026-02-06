using LearningAssistant.Enums;
using MediatR;

namespace LearningAssistant.Features.StreamLesson.V1;

public record StreamAILessonCommand(Guid UserId, string Topic, DifficultyLevel Level, string? ModelId = null) : IStreamRequest<string>;


