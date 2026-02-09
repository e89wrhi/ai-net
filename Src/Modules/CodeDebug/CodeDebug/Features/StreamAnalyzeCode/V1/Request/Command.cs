using AI.Common.Core;
using CodeDebug.Enums;
using MediatR;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public record StreamAnalyzeCodeCommand(Guid UserId, string Code, ProgrammingLanguage Language, DebugDepth Depth, bool IncludeSuggestion, string? ModelId = null) : IStreamRequest<string>;

