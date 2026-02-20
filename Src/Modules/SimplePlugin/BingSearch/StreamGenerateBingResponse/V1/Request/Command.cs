using AI.Common.Core;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public record StreamGenerateBingResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<StreamGenerateBingResponseCommandResult>;

public record StreamGenerateBingResponseCommandResult(
    IAsyncEnumerable<string> TextStream,
    string ModelId,
    string? ProviderName);
