namespace ChatBot.Features.GenerateResponse.V1;

public record GenerateResponseRequestDto(Guid SessionId, string? ModelId = null);
public record GenerateResponseResponseDto(Guid MessageId, string Content, string ModelId, string? ProviderName);
