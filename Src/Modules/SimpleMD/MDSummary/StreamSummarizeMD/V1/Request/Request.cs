namespace SimpleMD.Features.StreamSummarizeMD.V1;

public record StreamSummarizeMDRequestDto(string Text, string? ModelId = null);
public record StreamSummarizeMDResponseDto(string Response, string ModelId, string? ProviderName);
