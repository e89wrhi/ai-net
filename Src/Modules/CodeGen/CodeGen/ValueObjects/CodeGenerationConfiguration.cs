using AiOrchestration.ValueObjects;
using CodeGen.Enums;
using CodeGen.Exceptions;

namespace CodeGen.ValueObjects;

public record CodeGenerationConfiguration
{
    public Temperature Temperature { get; }
    public TokenCount MaxTokens { get; }
    public CodeStyle Style { get; }
    public bool IncludeComments { get; }

    public CodeGenerationConfiguration(
        Temperature temperature,
        TokenCount maxTokens,
        CodeStyle style,
        bool includeComments)
    {
        Temperature = temperature;
        MaxTokens = maxTokens;
        Style = style;
        IncludeComments = includeComments;
    }
}
