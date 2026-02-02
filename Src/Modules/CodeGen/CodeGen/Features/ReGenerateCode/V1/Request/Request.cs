namespace CodeGen.Features.ReGenerateCode.V1;

public record ReGenerateCodeRequestDto(Guid SessionId, string Instruction);
public record ReGenerateCodeResponseDto(Guid ResultId, string Code);
