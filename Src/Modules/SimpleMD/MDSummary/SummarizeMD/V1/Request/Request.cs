namespace SimpleMD.Features.SummarizeMD.V1;

/// <summary>
/// <paramref name="Instruction"/> tells the AI how to format the summary,
/// e.g. "bullet points", "one paragraph", "executive summary".
/// </summary>
public record SummarizeMDRequestDto(string Instruction, string? ModelId = null);
public record SummarizeMDResponseDto(string Response, string ModelId, string? ProviderName);
