namespace ChatBot.Features.StartChat.V1;


public record StartChatRequest(Guid UserId, string Title, string AiModelId);

public record StartChatRequestResponse(Guid Id);
