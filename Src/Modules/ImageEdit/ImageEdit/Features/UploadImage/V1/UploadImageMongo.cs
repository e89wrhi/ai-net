namespace ImageEdit.Features.StartImageEdit.V1;


using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using ImageEdit.Exceptions;
using System;

public record UploadImageMongo(Guid Id, Guid UserId, string Title, string AiModelId, string SessionStatus, DateTime CreatedAt) : InternalCommand;

public class StartImageEditMongoHandler : ICommandHandler<UploadImageMongo>
{
    private readonly ImageEditReadDbContext _readDbContext;

    public StartImageEditMongoHandler(ImageEditReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UploadImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedit = new ImageEditReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Title = request.Title,
            AiModelId = request.AiModelId,
            SessionStatus = request.SessionStatus,
            LastSentAt = request.CreatedAt,
            ImageEdits = new List<ImageEditReadModel>()
        };

        await _readDbContext.ImageEdits.InsertOneAsync(imageedit, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

