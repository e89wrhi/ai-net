namespace SimpleMD.Features.StreamGenerateResponse.V1;

public record StreamGenerateResponseRequestDto(string Text, string? ModelId = null);
public record StreamGenerateResponseResponseDto(string Response, string ModelId, string? ProviderName);
