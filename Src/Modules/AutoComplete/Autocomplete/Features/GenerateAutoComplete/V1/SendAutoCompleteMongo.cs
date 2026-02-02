using AI.Common.Core;
using AutoComplete.Data;
using AutoComplete.Models;
using MediatR;
using MongoDB.Driver;
using Ardalis.GuardClauses;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public record SendAutoCompleteMongo(
    Guid SessionId, 
    Guid RequestId, 
    string Prompt, 
    string Response, 
    int TokensUsed) : InternalCommand;

public class SendAutoCompleteMongoHandler : ICommandHandler<SendAutoCompleteMongo>
{
    private readonly AutocompleteReadDbContext _readDbContext;

    public SendAutoCompleteMongoHandler(AutocompleteReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SendAutoCompleteMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<AutoCompleteSessionReadModel>.Filter.Eq(x => x.Id, request.SessionId);
        
        var userMessage = new AutoCompleteReadModel
        {
            Id = Guid.NewGuid(),
            Content = request.Prompt,
            Sender = "User",
            SentAt = DateTime.UtcNow, // Ideally passed from event
            TokenUsed = 0 // Prompt tokens?
        };

        var aiMessage = new AutoCompleteReadModel
        {
            Id = request.RequestId, // Use RequestId for AI message or generic
            Content = request.Response,
            Sender = "AI",
            SentAt = DateTime.UtcNow,
            TokenUsed = request.TokensUsed
        };

        var update = Builders<AutoCompleteSessionReadModel>.Update
            .Push(x => x.AutoCompletes, userMessage)
            .Push(x => x.AutoCompletes, aiMessage)
            .Inc(x => x.TotalTokens, request.TokensUsed);

        // We should ideally use PushEach but driver support varies or syntax.
        // Chained Push might work or use PushEach.
        // Simple update: separate pushes might result in race/array issues?
        // Better:
        // .PushEach(x => x.AutoCompletes, new[] { userMessage, aiMessage })

        var updateCombined = Builders<AutoCompleteSessionReadModel>.Update
            .PushEach(x => x.AutoCompletes, new[] { userMessage, aiMessage })
            .Inc(x => x.TotalTokens, request.TokensUsed)
            .Set(x => x.LastSentAt, DateTime.UtcNow);

        await _readDbContext.AutoCompletes.UpdateOneAsync(filter, updateCombined, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
