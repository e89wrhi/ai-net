using MediatR;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;


public record StreamAnalyzeCodeCommand(string Code, ProgrammingLanguage Language) : IStreamRequest<string>;
