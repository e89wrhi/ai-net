using AI.Common.Core;

namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public record StreamGenerateBingResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<StreamGenerateBingResponseCommandResult>;

public record StreamGenerateBingResponseCommandResult(string Response, string ModelId, string? ProviderName);
