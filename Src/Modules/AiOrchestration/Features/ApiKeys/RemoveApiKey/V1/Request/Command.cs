using AI.Common.Core;

namespace AiOrchestration.Features.ApiKeys.RemoveApiKey.V1.Request;

public record RemoveApiKeyCommand(Guid Id) : ICommand;
