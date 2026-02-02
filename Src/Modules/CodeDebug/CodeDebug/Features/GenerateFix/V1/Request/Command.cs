using AI.Common.Core;

namespace CodeDebug.Features.GenerateFix.V1;

public record GenerateFixCommand(Guid SessionId, Guid ReportId) : ICommand<GenerateFixCommandResult>;

public record GenerateFixCommandResult(string FixedCode, string Explanation);
