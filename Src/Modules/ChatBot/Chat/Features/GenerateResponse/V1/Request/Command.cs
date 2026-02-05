using AI.Common.Core;

namespace ChatBot.Features.GenerateResponse.V1;

public record GenerateAiResponseCommand(Guid SessionId, string? ModelId = null) : ICommand<GenerateAiResponseCommandResult>;

public record GenerateAiResponseCommandResult(Guid MessageId, string Content, string ModelId, string? ProviderName);
