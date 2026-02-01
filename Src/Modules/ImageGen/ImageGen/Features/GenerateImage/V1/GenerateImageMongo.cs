namespace ImageGen.Features.DeleteImageGen.V1;

using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using ImageGen.Exceptions;
using System;

public record GenerateImageMongo(Guid ImageGenId) : InternalCommand;

public class DeleteImageGenMongoHandler : ICommandHandler<GenerateImageMongo>
{
    private readonly ImageGenReadDbContext _readDbContext;

    public DeleteImageGenMongoHandler(ImageGenReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ImageGenerationReadModel>.Filter.Eq(x => x.Id, request.ImageGenId);

        await _readDbContext.ImageGens.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

