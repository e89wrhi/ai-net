using LearningAssistant.Enums;
using MediatR;

namespace LearningAssistant.Features.StreamAILesson.V1;

public record StreamAILessonCommand(string Topic, DifficultyLevel Level) : IStreamRequest<string>;

