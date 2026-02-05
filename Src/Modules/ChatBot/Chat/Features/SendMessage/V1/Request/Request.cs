namespace ChatBot.Features.SendMessage.V1;

public record SendMessageRequest(Guid SessionId, string Content);
public record SendMessageRequestResponse(Guid MessageId);
