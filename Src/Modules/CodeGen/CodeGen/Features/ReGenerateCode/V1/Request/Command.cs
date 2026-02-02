using AI.Common.Core;

namespace CodeGen.Features.ReGenerateCode.V1;


public record ReGenerateCodeCommand(Guid SessionId, string Instruction) : ICommand<ReGenerateCodeCommandResult>;

public record ReGenerateCodeCommandResult(Guid ResultId, string Code);
