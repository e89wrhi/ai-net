namespace SpeechToText.Features.DeleteSpeechToText.V1;

using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using SpeechToText.Exceptions;
using System;

public record UploadSpeechAudioMongo(Guid SpeechToTextId) : InternalCommand;

public class DeleteSpeechToTextMongoHandler : ICommandHandler<UploadSpeechAudioMongo>
{
    private readonly SpeechToTextReadDbContext _readDbContext;

    public DeleteSpeechToTextMongoHandler(SpeechToTextReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UploadSpeechAudioMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<SpeechToTextSessionReadModel>.Filter.Eq(x => x.Id, request.SpeechToTextId);

        await _readDbContext.SpeechToTexts.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

