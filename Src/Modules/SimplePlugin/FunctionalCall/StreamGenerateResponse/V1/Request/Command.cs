using AI.Common.Core;

namespace SimplePlugin.Features.StreamGenerateResponse.V1;

public record StreamGenerateResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<StreamGenerateResponseCommandResult>;

public record StreamGenerateResponseCommandResult(string Response, string ModelId, string? ProviderName);
