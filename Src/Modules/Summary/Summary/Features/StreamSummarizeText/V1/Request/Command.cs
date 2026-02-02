using MediatR;
using Summary.Enums;

namespace Summary.Features.StreamSummarizeText.V1;

public record StreamSummarizeTextCommand(string Text, SummaryDetailLevel DetailLevel, string Language) : IStreamRequest<string>;
