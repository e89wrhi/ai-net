using AI.Common.Core;

namespace CodeGen.Features.GenerateCode.V1;

public record GenerateCodeCommand(Guid UserId, string Prompt, string Language, string? ModelId = null) : ICommand<GenerateCodeCommandResult>;

public record GenerateCodeCommandResult(Guid SessionId, Guid ResultId, string Code, string ModelId, string? ProviderName);
