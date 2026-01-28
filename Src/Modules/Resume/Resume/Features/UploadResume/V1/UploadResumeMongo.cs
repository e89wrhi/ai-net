namespace Resume.Features.UploadResume.V1;


using Ardalis.GuardClauses;
using Resume.Data;
using Resume.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Resume.Exceptions;
using System;

public record UploadResumeMongo(Guid Id, string UserId, string CandidateName, string FilePath, string Status, DateTime CreatedAt) : InternalCommand;

public class UploadResumeMongoHandler : ICommandHandler<UploadResumeMongo>
{
    private readonly ResumeReadDbContext _readDbContext;

    public UploadResumeMongoHandler(ResumeReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(UploadResumeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var resume = new ResumeReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            CandidateName = request.CandidateName,
            FilePath = request.FilePath,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            Summary = string.Empty,
            ParsedText = string.Empty,
            Skills = new List<string>(),
            Suggestions = new List<string>()
        };

        await _readDbContext.Resume.InsertOneAsync(resume, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

