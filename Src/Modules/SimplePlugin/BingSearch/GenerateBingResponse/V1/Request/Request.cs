namespace SimplePlugin.Features.GenerateBingResponse.V1;

public record GenerateBingResponseRequestDto(string Text, string? ModelId = null);
public record GenerateBingResponseResponseDto(string Response, string ModelId, string? ProviderName);
