using AI.Common.Core;

namespace SimpleMD.Features.SummarizeMD.V1;

/// <summary>
/// <paramref name="Instruction"/> tells the AI how to format the summary,
/// e.g. "bullet points", "one paragraph", "executive summary".
/// </summary>
public record SummarizeMDCommand(Guid UserId, string Instruction, string? ModelId = null) : ICommand<SummarizeMDCommandResult>;

public record SummarizeMDCommandResult(string Response, string ModelId, string? ProviderName);
