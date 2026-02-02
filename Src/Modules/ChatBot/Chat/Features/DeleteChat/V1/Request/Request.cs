namespace ChatBot.Features.DeleteChat.V1;

public record DeleteChatRequest(Guid SessionId);
public record DeleteChatRequestResponse(Guid Id);
