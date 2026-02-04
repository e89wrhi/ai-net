using AI.Common.Core;
using CodeGen.Enums;

namespace CodeGen.Features.GenerateCode.V1;

public record GenerateCodeCommand(Guid UserId, string Prompt, string Language, CodeQualityLevel Quality, CodeStyle Style, bool IncludeComments, string? ModelId = null) : ICommand<GenerateCodeCommandResult>;

public record GenerateCodeCommandResult(Guid SessionId, Guid ResultId, string Code, string ModelId, string? ProviderName);
