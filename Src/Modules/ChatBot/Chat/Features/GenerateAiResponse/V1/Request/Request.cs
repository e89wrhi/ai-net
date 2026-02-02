namespace ChatBot.Features.GenerateAiResponse.V1;


public record GenerateAiResponseRequestDto(Guid SessionId);
public record GenerateAiResponseResponseDto(Guid MessageId, string Content);
