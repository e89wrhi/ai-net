using ImageCaption.Data;
using ImageCaption.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImageCaption.Features.UploadImage.V1;

internal class UploadImageHandler : IRequestHandler<UploadImageCommand, UploadImageCommandResponse>
{
    private readonly ImageCaptionDbContext _dbContext;

    public UploadImageHandler(ImageCaptionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadImageCommandResponse> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var image = ImageModel.Create(
            ImageCaptionResultId.Of(NewId.NextGuid()),
            request.UserId,
            ValueObjects.CaptionConfidence.Of(request.ImageUrl, request.FileName),
            request.Width,
            request.Height,
            request.SizeInBytes,
            request.Format);

        await _dbContext.Images.AddAsync(image, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UploadImageCommandResponse(image.Id.Value);
    }
}


