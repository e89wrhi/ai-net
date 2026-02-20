namespace SimplePlugin.Features.StreamGenerateResponse.V1;

public record StreamGenerateResponseRequestDto(string Text, string? ModelId = null);

/// <summary>
/// Sent as the first SSE 'meta' event.
/// </summary>
public record StreamGenerateResponseResponseDto(string ModelId, string? ProviderName);
