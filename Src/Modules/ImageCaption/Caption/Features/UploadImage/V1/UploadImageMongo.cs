namespace ImageCaption.Features.UploadImage.V1;


using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using ImageCaption.Exceptions;
using System;

public record UploadImageMongo(Guid Id, string UserId, string FilePath, string Status, int Width, int Height, long Size, string Format, DateTime UploadedAt) : InternalCommand;

public class UploadImageMongoHandler : ICommandHandler<UploadImageMongo>
{
    private readonly ImageCaptionReadDbContext _readDbContext;

    public UploadImageMongoHandler(ImageCaptionReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UploadImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var image = new ImageCaptionReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            FilePath = request.FilePath,
            Status = request.Status,
            Width = request.Width,
            Height = request.Height,
            SizeInBytes = request.Size,
            Format = request.Format,
            UploadedAt = request.UploadedAt
        };

        await _readDbContext.Image.InsertOneAsync(image, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

