using LearningAssistant.Enums;
using MediatR;

namespace LearningAssistant.Features.StreamLesson.V1;

public record StreamAILessonCommand(string Topic, DifficultyLevel Level) : IStreamRequest<string>;
