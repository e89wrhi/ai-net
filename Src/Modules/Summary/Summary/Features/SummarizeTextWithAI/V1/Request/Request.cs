using Summary.Enums;

namespace Summary.Features.SummarizeTextWithAI.V1;


public record SummarizeTextWithAIRequestDto(string Text, SummaryDetailLevel DetailLevel, string Language);
public record SummarizeTextWithAIResponseDto(Guid SessionId, Guid ResultId, string Summary);
