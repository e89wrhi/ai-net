namespace SimpleMD.Features.GenerateResponse.V1;

public record GenerateResponseRequestDto(string Text, string? ModelId = null);
public record GenerateResponseResponseDto(string Response, string ModelId, string? ProviderName);
