using AI.Common.Core;

namespace ChatBot.Features.GenerateResponse.V1;

public record GenerateAiResponseCommand(Guid SessionId) : ICommand<GenerateAiResponseCommandResult>;

public record GenerateAiResponseCommandResult(Guid MessageId, string Content);
