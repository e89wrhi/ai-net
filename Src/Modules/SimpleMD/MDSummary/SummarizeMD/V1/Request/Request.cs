namespace SimpleMD.Features.SummarizeMD.V1;

public record SummarizeMDRequestDto(string Text, string? ModelId = null);
public record SummarizeMDResponseDto(string Response, string ModelId, string? ProviderName);
