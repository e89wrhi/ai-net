using AI.Common.Contracts.EventBus.AutoCompletes;
using AI.Common.Core;
using AutoComplete.Events;
using AutoComplete.Features.DeleteAutoComplete.V1;
using AutoComplete.Features.GenerateAutoComplete.V1;

namespace AutoComplete;

public sealed class AutoCompleteEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            AutoCompleteSessionStartedDomainEvent e => new AutoCompleteSessionStarted(e.Id.Value),
            // AutoCompleteRequestedDomainEvent e => new AutoCompleteRecieved(e.SessionId.Value), 
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            AutoCompleteSessionStartedDomainEvent e => new StartAutoCompleteMongo(e.Id.Value, e.UserId.Value, "New AutoComplete Session", e.AiModelId.Value, "Active", DateTime.UtcNow),
            AutoCompleteRequestedDomainEvent e => new SendAutoCompleteMongo(e.SessionId.Value, e.RequestId.Value, e.Prompt, e.Response, e.TokensUsed),
            AutoCompleteSessionDeletedDomainEvent e => new AutocompleteMongo(e.Id.Value),
            _ => null
        };
    }
}