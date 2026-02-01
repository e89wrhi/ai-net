namespace ImageGen.Features.SendImageGen.V1;

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

public record ReGenerateImageMongo(Guid ImageGenId, Guid ImageGenId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendImageGenMongoHandler : ICommandHandler<ReGenerateImageMongo>
{
    private readonly ImageGenReadDbContext _readDbContext;

    public SendImageGenMongoHandler(ImageGenReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReGenerateImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ImageGenerationReadModel>.Filter.Eq(x => x.Id, request.ImageGenId);
        
        var message = new ImageGenReadModel
        {
            Id = request.ImageGenId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<ImageGenerationReadModel>.Update
            .Push(x => x.ImageGens, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.ImageGens.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

