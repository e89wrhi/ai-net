namespace CodeGen.Features.ReGenerateCode.V1;

public record ReGenerateCodeRequestDto(Guid SessionId, string Instruction, string? ModelId = null);
public record ReGenerateCodeResponseDto(Guid ResultId, string Code, string ModelId, string? ProviderName);
