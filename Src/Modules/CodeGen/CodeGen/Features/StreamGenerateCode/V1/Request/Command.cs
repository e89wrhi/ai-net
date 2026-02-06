using MediatR;

namespace CodeGen.Features.StreamGenerateCode.V1;

public record StreamGenerateCodeCommand(Guid UserId, string Prompt, string Language, string? ModelId = null) : IStreamRequest<string>;



