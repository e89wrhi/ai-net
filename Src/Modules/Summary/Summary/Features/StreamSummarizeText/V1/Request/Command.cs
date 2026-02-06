using MediatR;
using Summary.Enums;

namespace Summary.Features.StreamSummarizeText.V1;

public record StreamSummarizeTextCommand(Guid UserId, string Text, SummaryDetailLevel DetailLevel, string Language, string? ModelId = null) : IStreamRequest<string>;


