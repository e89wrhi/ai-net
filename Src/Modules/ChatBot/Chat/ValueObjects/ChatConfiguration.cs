using AiOrchestration.ValueObjects;
using ChatBot.Models;

namespace ChatBot.ValueObjects;

public record ChatConfiguration
{
    public Temperature Temperature { get; }
    public TokenCount MaxTokens { get; }
    public SystemPrompt SystemPrompt { get; }

    public ChatConfiguration(
        Temperature temperature,
        TokenCount maxTokens,
        SystemPrompt systemPrompt)
    {
        Temperature = temperature;
        MaxTokens = maxTokens;
        SystemPrompt = systemPrompt;
    }
}
