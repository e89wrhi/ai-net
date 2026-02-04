namespace CodeGen.Features.GenerateCode.V1;

public record GenerateCodeRequestDto(string Prompt, string Language, string? ModelId = null);
public record GenerateCodeResponseDto(Guid SessionId, Guid ResultId, string Code, string ModelId, string? ProviderName);
