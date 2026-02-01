namespace ImageEdit.Features.SendImageEdit.V1;

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

public record ReEditImageMongo(Guid ImageEditId, Guid ImageEditId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendImageEditMongoHandler : ICommandHandler<ReEditImageMongo>
{
    private readonly ImageEditReadDbContext _readDbContext;

    public SendImageEditMongoHandler(ImageEditReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReEditImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ImageEditSessionReadModel>.Filter.Eq(x => x.Id, request.ImageEditId);
        
        var message = new ImageEditReadModel
        {
            Id = request.ImageEditId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<ImageEditSessionReadModel>.Update
            .Push(x => x.ImageEdits, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.ImageEdits.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

