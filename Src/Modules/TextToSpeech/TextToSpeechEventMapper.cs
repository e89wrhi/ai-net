using AI.Common.Contracts.EventBus.TextToSpeechs;
using AI.Common.Core;
using TextToSpeech.Events;
using TextToSpeech.Features.DeleteTextToSpeech.V1;
using TextToSpeech.Features.SendTextToSpeech.V1;
using TextToSpeech.Features.StartTextToSpeech.V1;

namespace TextToSpeech;

public sealed class TextToSpeechEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            TextToSpeechSessionStartedDomainEvent e => new TextToSpeechSessionStarted(e.SessionId.Value),
            TextSynthesizedDomainEvent e => new TextToSpeechRecieved(e.TextToSpeechId.Value),
            TextToSpeechRespondedDomainEvent e => new TextToSpeechResponded(e.TextToSpeechId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            TextToSpeechSessionStartedDomainEvent e => new StartTextToSpeechMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            TextSynthesizedDomainEvent e => new SendTextToSpeechMongo(e.SessionId.Value, e.TextToSpeechId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            TextToSpeechRespondedDomainEvent e => new SendTextToSpeechMongo(e.SessionId.Value, e.TextToSpeechId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            TextToSpeechSessionDeletedDomainEvent e => new GenerateSpeechMongo(e.Id.Value),
            _ => null
        };
    }
}