using AI.Common.Core;

namespace ChatBot.Features.UpdateChat.V1;

public record UpdateChatCommand(Guid SessionId, Guid UserId, string Title) : ICommand<UpdateChatCommandResponse>;

public record UpdateChatCommandResponse(bool Success);

public record UpdateChatRequest(string Title);

public record UpdateChatRequestResponse(bool Success);
