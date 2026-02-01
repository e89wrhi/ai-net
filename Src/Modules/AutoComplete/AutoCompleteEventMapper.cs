using AI.Common.Contracts.EventBus.AutoCompletes;
using AI.Common.Core;
using AutoComplete.Events;
using AutoComplete.Features.DeleteAutoComplete.V1;
using AutoComplete.Features.SendAutoComplete.V1;
using AutoComplete.Features.StartAutoComplete.V1;

namespace AutoComplete;

public sealed class AutoCompleteEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            AutoCompleteSessionStartedDomainEvent e => new AutoCompleteSessionStarted(e.SessionId.Value),
            AutoCompleteRecievedDomainEvent e => new AutoCompleteRecieved(e.AutoCompleteId.Value),
            AutoCompleteRespondedDomainEvent e => new AutoCompleteResponded(e.AutoCompleteId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            AutoCompleteSessionStartedDomainEvent e => new StartAutoCompleteMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            AutoCompleteRecievedDomainEvent e => new SendAutoCompleteMongo(e.SessionId.Value, e.AutoCompleteId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            AutoCompleteRespondedDomainEvent e => new SendAutoCompleteMongo(e.SessionId.Value, e.AutoCompleteId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            AutoCompleteSessionDeletedDomainEvent e => new AutocompleteMongo(e.Id.Value),
            _ => null
        };
    }
}