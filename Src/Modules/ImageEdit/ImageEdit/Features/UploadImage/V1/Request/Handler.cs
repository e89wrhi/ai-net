using ImageEdit.Data;
using MassTransit;
using MediatR;

namespace ImageEdit.Features.UploadImage.V1;

internal class StartImageEditHandler : IRequestHandler<StartImageEditCommand, StartImageEditCommandResponse>
{
    private readonly ImageEditDbContext _dbContext;

    public StartImageEditHandler(ImageEditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartImageEditCommandResponse> Handle(StartImageEditCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedit = ImageEditModel.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.ImageEdits.AddAsync(imageedit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StartImageEditCommandResponse(imageedit.Id.Value);
    }
}


