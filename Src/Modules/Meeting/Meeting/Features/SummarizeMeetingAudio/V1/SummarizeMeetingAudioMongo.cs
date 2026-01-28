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

public record SummarizeMeetingAudioMongo() : InternalCommand;

public class SummarizeMeetingAudioMongoHandler : ICommandHandler<SummarizeMeetingAudioMongo>
{
    private readonly MeetingReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public SummarizeMeetingAudioMongoHandler(
        MeetingReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(SummarizeMeetingAudioMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<MeetingReadModel>(request);


        return Unit.Value;
    }
}
