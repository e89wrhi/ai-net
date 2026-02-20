using AI.Common.Core;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

/// <summary>
/// <paramref name="Instruction"/> tells the AI how to format the summary,
/// e.g. "bullet points", "one paragraph", "executive summary".
/// </summary>
public record StreamSummarizeMDCommand(Guid UserId, string Instruction, string? ModelId = null) : ICommand<StreamSummarizeMDCommandResult>;

/// <summary>
/// Holds a lazy async stream of summarized text chunks plus model metadata.
/// </summary>
public record StreamSummarizeMDCommandResult(
    IAsyncEnumerable<string> TextStream,
    string ModelId,
    string? ProviderName);
