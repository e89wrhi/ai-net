using CodeDebug.Enums;
using MediatR;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public record StreamAnalyzeCodeCommand(Guid UserId, string Code, ProgrammingLanguage Language, string? ModelId = null) : IStreamRequest<string>;


