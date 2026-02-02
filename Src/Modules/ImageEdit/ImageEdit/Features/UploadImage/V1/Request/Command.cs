using AI.Common.Core;
using MassTransit;

namespace ImageEdit.Features.UploadImage.V1;

public record StartImageEditCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartImageEditCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartImageEditCommandResponse(Guid Id);

