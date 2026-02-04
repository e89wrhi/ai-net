using AI.Common.Core;

namespace CodeDebug.Features.GenerateFix.V1;

public record GenerateFixCommand(Guid SessionId, Guid ReportId, string? ModelId = null) : ICommand<GenerateFixCommandResult>;

public record GenerateFixCommandResult(string FixedCode, string Explanation, string ModelId, string? ProviderName);
