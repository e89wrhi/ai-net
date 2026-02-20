using AI.Common.Core;

namespace SimpleMD.Features.StreamGenerateResponse.V1;

public record StreamGenerateResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<StreamGenerateResponseCommandResult>;

/// <summary>
/// Holds a lazy async stream of text chunks from the AI model, plus model metadata resolved
/// after the first chunk is received. The stream is consumed exactly once by the endpoint.
/// </summary>
public record StreamGenerateResponseCommandResult(
    IAsyncEnumerable<string> TextStream,
    string ModelId,
    string? ProviderName);
