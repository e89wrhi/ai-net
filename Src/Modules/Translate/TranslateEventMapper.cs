using AI.Common.Contracts.EventBus.Translates;
using AI.Common.Core;
using Translate.Events;
using Translate.Features.DeleteTranslate.V1;
using Translate.Features.SendTranslate.V1;
using Translate.Features.StartTranslate.V1;

namespace Translate;

public sealed class TranslateEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            TranslationSessionStartedDomainEvent e => new TranslateSessionStarted(e.SessionId.Value),
            TranslateRecievedDomainEvent e => new TranslateRecieved(e.TranslateId.Value),
            TranslateRespondedDomainEvent e => new TranslateResponded(e.TranslateId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            TranslationSessionStartedDomainEvent e => new StartTranslateMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            TranslateRecievedDomainEvent e => new SendTranslateMongo(e.SessionId.Value, e.TranslateId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            TranslateRespondedDomainEvent e => new SendTranslateMongo(e.SessionId.Value, e.TranslateId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            TranslateSessionDeletedDomainEvent e => new TranslateTextMongo(e.Id.Value),
            _ => null
        };
    }
}