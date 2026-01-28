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

public record GenerateCaptionMongo() : InternalCommand;

public class GenerateCaptionMongoHandler : ICommandHandler<GenerateCaptionMongo>
{
    private readonly ImageReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public GenerateCaptionMongoHandler(
        ImageReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(GenerateCaptionMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ImageReadModel>(request);


        return Unit.Value;
    }
}
