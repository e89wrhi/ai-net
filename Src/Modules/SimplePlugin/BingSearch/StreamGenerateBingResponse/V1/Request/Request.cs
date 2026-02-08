namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public record StreamGenerateBingResponseRequestDto(string Text, string? ModelId = null);
public record StreamGenerateBingResponseResponseDto(string Response, string ModelId, string? ProviderName);
