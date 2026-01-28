namespace Meeting.Features.UploadMeetingAudio.V1;


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

public record UploadMeetingAudioMongo(Guid Id, string OrganizerId, string Title, string Status, DateTime CreatedAt) : InternalCommand;

public class UploadMeetingAudioMongoHandler : ICommandHandler<UploadMeetingAudioMongo>
{
    private readonly MeetingReadDbContext _readDbContext;

    public UploadMeetingAudioMongoHandler(MeetingReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UploadMeetingAudioMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var meeting = new MeetingReadModel
        {
            Id = request.Id,
            OrganizerId = request.OrganizerId,
            Title = request.Title,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            Summary = string.Empty,
            Transcript = string.Empty,
            ActionItems = new List<string>()
        };

        await _readDbContext.Meeting.InsertOneAsync(meeting, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

