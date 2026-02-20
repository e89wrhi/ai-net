namespace SimpleMD.Features.StreamSummarizeMD.V1;

/// <summary>
/// <paramref name="Instruction"/> tells the AI how to format the summary,
/// e.g. "bullet points", "one paragraph", "executive summary".
/// </summary>
public record StreamSummarizeMDRequestDto(string Instruction, string? ModelId = null);

/// <summary>
/// Header event sent before the SSE stream begins, containing model metadata.
/// </summary>
public record StreamSummarizeMDResponseDto(string ModelId, string? ProviderName);
