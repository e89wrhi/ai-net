using CodeGen.Enums;

namespace CodeGen.Features.StreamGenerateCode.V1;

public record StreamGenerateCodeRequestDto(Guid UserId, string Prompt, string Language, CodeQualityLevel Quality, CodeStyle Style, bool IncludeComments, string? ModelId = null);

