using ImageCaption.Data;
using ImageCaption.Exceptions;
using ImageCaption.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ImageCaption.Features.GenerateCaption.V1;


internal class GenerateCaptionHandler : IRequestHandler<GenerateCaptionCommand, GenerateCaptionCommandResponse>
{
    private readonly ImageCaptionDbContext _dbContext;

    public GenerateCaptionHandler(ImageCaptionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateCaptionCommandResponse> Handle(GenerateCaptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var image = await _dbContext.Images.FindAsync(new object[] { ImageCaptionResultId.Of(request.ImageId) }, cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(request.ImageId);
        }

        var caption = CaptionModel.Create(
            ImageCaptionId.Of(NewId.NextGuid()),
            image.Id,
            request.CaptionText,
            request.Confidence,
            request.Language);

        image.AddCaption(caption);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateCaptionCommandResponse(caption.Id.Value);
    }
}

