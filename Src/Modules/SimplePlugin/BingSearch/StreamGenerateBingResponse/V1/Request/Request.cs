namespace SimplePlugin.Features.StreamGenerateBingResponse.V1;

public record StreamGenerateBingResponseRequestDto(string Text, string? ModelId = null);

/// <summary>
/// Sent as the first SSE 'meta' event before streaming begins.
/// </summary>
public record StreamGenerateBingResponseResponseDto(string ModelId, string? ProviderName);
