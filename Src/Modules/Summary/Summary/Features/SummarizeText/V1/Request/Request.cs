using Summary.Enums;

namespace Summary.Features.SummarizeText.V1;

public record SummarizeTextRequestDto(string Text, SummaryDetailLevel DetailLevel, string Language, string? ModelId = null);
public record SummarizeTextResponseDto(Guid SessionId, Guid ResultId, string Summary, string ModelId, string? ProviderName);
