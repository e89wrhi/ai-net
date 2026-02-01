namespace ImageEdit.Features.DeleteImageEdit.V1;

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

public record EditImageMongo(Guid ImageEditId) : InternalCommand;

public class DeleteImageEditMongoHandler : ICommandHandler<EditImageMongo>
{
    private readonly ImageEditReadDbContext _readDbContext;

    public DeleteImageEditMongoHandler(ImageEditReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(EditImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ImageEditSessionReadModel>.Filter.Eq(x => x.Id, request.ImageEditId);

        await _readDbContext.ImageEdits.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

