using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeGen.Enums;
using CodeGen.ValueObjects;

namespace CodeGen.Models;

public record CodeGenerationResult : Entity<CodeGenResultId>
{
    public CodeGenerationPrompt Prompt { get; private set; } = default!;
    public GeneratedCode Code { get; private set; } = default!;
    public ProgrammingLanguage Language { get; private set; }
    public CodeQualityLevel QualityLevel { get; private set; }
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime GeneratedAt { get; private set; }

    private CodeGenerationResult() { }

    public static CodeGenerationResult Create(
        CodeGenResultId id,
        CodeGenerationPrompt prompt,
        GeneratedCode code,
        ProgrammingLanguage language,
        CodeQualityLevel qualityLevel,
        TokenCount tokenUsed,
        CostEstimate cost)
    {
        return new CodeGenerationResult
        {
            Id = id,
            Prompt = prompt,
            Code = code,
            Language = language,
            QualityLevel = qualityLevel,
            TokenUsed = tokenUsed,
            Cost = cost,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
