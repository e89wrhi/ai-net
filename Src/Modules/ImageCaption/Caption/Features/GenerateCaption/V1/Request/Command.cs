using AI.Common.Core;
using MassTransit;

namespace ImageCaption.Features.GenerateCaption.V1;


public record GenerateCaptionCommand(Guid ImageId, string CaptionText, double Confidence, string Language) : ICommand<GenerateCaptionCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateCaptionCommandResponse(Guid Id);
