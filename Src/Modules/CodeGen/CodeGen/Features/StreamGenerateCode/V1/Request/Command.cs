using CodeGen.Enums;
using MediatR;

namespace CodeGen.Features.StreamGenerateCode.V1;

public record StreamGenerateCodeCommand(Guid UserId, string Prompt, string Language, CodeQualityLevel Quality, CodeStyle Style, bool IncludeComments, string? ModelId = null) : IStreamRequest<string>;



