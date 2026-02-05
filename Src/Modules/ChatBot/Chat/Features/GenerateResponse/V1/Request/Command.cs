using AI.Common.Core;

namespace ChatBot.Features.GenerateResponse.V1;

public record GenerateAiResponseCommand(Guid UserId, Guid SessionId, string Message, string? ModelId = null) : ICommand<GenerateAiResponseCommandResult>;

public record GenerateAiResponseCommandResult(Guid MessageId, string Content, string ModelId, string? ProviderName);
