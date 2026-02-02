using AI.Common.Core;

namespace ChatBot.Features.DeleteChat.V1;

public record DeleteChatCommand(Guid SessionId) : ICommand<DeleteChatCommandResponse>;

public record DeleteChatCommandResponse(Guid Id);
