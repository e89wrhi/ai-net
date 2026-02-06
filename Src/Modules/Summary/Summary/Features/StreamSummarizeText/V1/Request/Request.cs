using Summary.Enums;

namespace Summary.Features.StreamSummarizeText.V1;

public record StreamSummarizeTextRequestDto(string Text, SummaryDetailLevel DetailLevel, string Language, string? ModelId = null);

