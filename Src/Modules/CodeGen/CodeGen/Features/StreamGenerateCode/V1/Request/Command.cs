using MediatR;

namespace CodeGen.Features.StreamGenerateCode.V1;

public record StreamGenerateCodeCommand(string Prompt, string Language) : IStreamRequest<string>;

