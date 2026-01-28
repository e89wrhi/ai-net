namespace ImageCaption.Features.GenerateCaption.V1;


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

public record GenerateCaptionMongo(Guid ImageId, string Text, string Status, DateTime ProcessedAt) : InternalCommand;

public class GenerateCaptionMongoHandler : ICommandHandler<GenerateCaptionMongo>
{
    private readonly ImageReadDbContext _readDbContext;

    public GenerateCaptionMongoHandler(ImageReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateCaptionMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ImageReadModel>.Filter.Eq(x => x.Id, request.ImageId);
        
        var caption = new CaptionReadModel
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            CreatedAt = request.ProcessedAt
        };

        var update = Builders<ImageReadModel>.Update
            .Push(x => x.Captions, caption)
            .Set(x => x.Status, request.Status);

        await _readDbContext.Image.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

