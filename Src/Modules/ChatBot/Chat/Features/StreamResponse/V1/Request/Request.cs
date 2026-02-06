namespace ChatBot.Features.StreamResponse.V1;

public record StreamAiResponseRequestDto(Guid SessionId, string? ModelId = null);

