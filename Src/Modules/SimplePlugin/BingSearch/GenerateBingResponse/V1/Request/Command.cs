using AI.Common.Core;

namespace SimplePlugin.Features.GenerateBingResponse.V1;

public record GenerateBingResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<GenerateBingResponseCommandResult>;

public record GenerateBingResponseCommandResult(string Response, string ModelId, string? ProviderName);
