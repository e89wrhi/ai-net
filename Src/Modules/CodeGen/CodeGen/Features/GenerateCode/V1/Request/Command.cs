using AI.Common.Core;

namespace CodeGen.Features.GenerateCode.V1;

public record GenerateCodeCommand(string Prompt, string Language) : ICommand<GenerateCodeCommandResult>;

public record GenerateCodeCommandResult(Guid SessionId, Guid ResultId, string Code);
