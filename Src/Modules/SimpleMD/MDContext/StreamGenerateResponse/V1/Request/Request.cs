namespace SimpleMD.Features.StreamGenerateResponse.V1;

public record StreamGenerateResponseRequestDto(string Text, string? ModelId = null);

/// <summary>
/// Header sent before the SSE stream begins, containing model metadata.
/// The actual content arrives as a sequence of plain-text SSE data lines.
/// </summary>
public record StreamGenerateResponseResponseDto(string ModelId, string? ProviderName);
