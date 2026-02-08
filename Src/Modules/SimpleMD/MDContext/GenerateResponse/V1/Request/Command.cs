using AI.Common.Core;

namespace SimpleMD.Features.GenerateResponse.V1;

public record GenerateResponseCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<GenerateResponseCommandResult>;

public record GenerateResponseCommandResult(string Response, string ModelId, string? ProviderName);
