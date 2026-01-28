namespace ImageCaption.Features.UploadImage.V1;


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

public record UploadImageMongo() : InternalCommand;

public class UploadImageMongoHandler : ICommandHandler<UploadImageMongo>
{
    private readonly ImageReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public UploadImageMongoHandler(
        ImageReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UploadImageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ImageReadModel>(request);


        return Unit.Value;
    }
}
