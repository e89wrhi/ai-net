using AI.Common.Core;
using AutoComplete.Data;
using AutoComplete.Models;
using MediatR;
using MongoDB.Driver;
using Ardalis.GuardClauses;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record StartAutoCompleteMongo(
    Guid SessionId, 
    Guid UserId, 
    string Title, 
    string AiModelId, 
    string Status, 
    DateTime CreatedAt) : InternalCommand;

public class StartAutoCompleteMongoHandler : ICommandHandler<StartAutoCompleteMongo>
{
    private readonly AutocompleteReadDbContext _readDbContext;

    public StartAutoCompleteMongoHandler(AutocompleteReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(StartAutoCompleteMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var readModel = new AutoCompleteSessionReadModel
        {
            Id = request.SessionId,
            UserId = request.UserId,
            Title = request.Title,
            AiModelId = request.AiModelId,
            SessionStatus = request.Status,
            LastSentAt = request.CreatedAt,
            TotalTokens = 0,
            AutoCompletes = new List<AutoCompleteReadModel>()
        };

        await _readDbContext.AutoCompletes.InsertOneAsync(readModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
