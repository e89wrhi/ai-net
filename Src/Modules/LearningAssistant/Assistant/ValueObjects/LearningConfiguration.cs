using LearningAssistant.Enums;

namespace LearningAssistant.ValueObjects;

public record LearningConfiguration
{
    public LearningMode Mode { get; }
    public DifficultyLevel Difficulty { get; }

    public LearningConfiguration(LearningMode mode, DifficultyLevel difficulty)
    {
        Mode = mode;
        Difficulty = difficulty;
    }
}
