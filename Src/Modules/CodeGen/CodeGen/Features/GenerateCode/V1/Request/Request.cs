namespace CodeGen.Features.GenerateCode.V1;

public record GenerateCodeRequestDto(string Prompt, string Language);
public record GenerateCodeResponseDto(Guid SessionId, Guid ResultId, string Code);
