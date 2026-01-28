namespace ChatBot.Features.DeleteChat.V1;

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

public record DeleteChatMongo() : InternalCommand;

public class DeleteChatMongoHandler : ICommandHandler<DeleteChatMongo>
{
    private readonly ChatReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public DeleteChatMongoHandler(
        ChatReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteChatMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ChatReadModel>(request);


        return Unit.Value;
    }
}
