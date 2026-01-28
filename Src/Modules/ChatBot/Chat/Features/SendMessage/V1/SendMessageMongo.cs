namespace ChatBot.Features.SendMessage.V1;

using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using ChatBot.Exceptions;
using System;

public record SendMessageMongo() : InternalCommand;

public class SendMessageMongoHandler : ICommandHandler<SendMessageMongo>
{
    private readonly ChatReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public SendMessageMongoHandler(
        ChatReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(SendMessageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ChatReadModel>(request);


        return Unit.Value;
    }
}
