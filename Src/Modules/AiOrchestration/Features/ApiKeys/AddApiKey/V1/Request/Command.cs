using AI.Common.Core;

namespace AiOrchestration.Features.ApiKeys.AddApiKey.V1.Request;

public record AddApiKeyCommand(string Provider, string Key, string Label) : ICommand<AddApiKeyCommandResult>;

public record AddApiKeyCommandResult(Guid Id, string Provider, string Label);
