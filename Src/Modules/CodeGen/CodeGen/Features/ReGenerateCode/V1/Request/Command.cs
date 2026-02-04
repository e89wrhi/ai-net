using AI.Common.Core;

namespace CodeGen.Features.ReGenerateCode.V1;

public record ReGenerateCodeCommand(Guid UserId, Guid SessionId, string Instruction, string? ModelId = null) : ICommand<ReGenerateCodeCommandResult>;

public record ReGenerateCodeCommandResult(Guid ResultId, string Code, string ModelId, string? ProviderName);
