namespace Meeting.Features.SummarizeMeetingAudio.V1;


using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Meeting.Exceptions;
using System;

public record SummarizeMeetingAudioMongo(Guid MeetingId, string Transcript, string Summary, string Status) : InternalCommand;

public class SummarizeMeetingAudioMongoHandler : ICommandHandler<SummarizeMeetingAudioMongo>
{
    private readonly MeetingReadDbContext _readDbContext;

    public SummarizeMeetingAudioMongoHandler(MeetingReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SummarizeMeetingAudioMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<MeetingAnalysisSessionReadModel>.Filter.Eq(x => x.Id, request.MeetingId);
        
        var update = Builders<MeetingAnalysisSessionReadModel>.Update
            .Set(x => x.Transcript, request.Transcript)
            .Set(x => x.Summary, request.Summary)
            .Set(x => x.Status, request.Status);

        await _readDbContext.Meeting.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

