namespace ChatBot.Features.SendMessage.V1;


public record SendMessageRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendMessageRequestResponse(Guid Id);
