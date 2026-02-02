using AI.Common.Core;
using MassTransit;

namespace ImageCaption.Features.UploadImage.V1;

public record UploadImageCommand(string UserId, string ImageUrl, string FileName, int Width, int Height, long SizeInBytes, string Format) : ICommand<UploadImageCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record UploadImageCommandResponse(Guid Id);

