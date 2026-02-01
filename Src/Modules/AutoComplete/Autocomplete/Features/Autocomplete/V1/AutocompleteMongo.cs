namespace AutoComplete.Features.DeleteAutoComplete.V1;

using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using AutoComplete.Exceptions;
using System;

public record AutocompleteMongo(Guid AutoCompleteId) : InternalCommand;

public class DeleteAutoCompleteMongoHandler : ICommandHandler<AutocompleteMongo>
{
    private readonly AutocompleteReadDbContext _readDbContext;

    public DeleteAutoCompleteMongoHandler(AutocompleteReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(AutocompleteMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<AutoCompleteSessionReadModel>.Filter.Eq(x => x.Id, request.AutoCompleteId);

        await _readDbContext.AutoCompletes.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

