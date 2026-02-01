using AI.Common.Contracts.EventBus.SpeechToTexts;
using AI.Common.Core;
using SpeechToText.Events;
using SpeechToText.Features.DeleteSpeechToText.V1;
using SpeechToText.Features.SendSpeechToText.V1;
using SpeechToText.Features.StartSpeechToText.V1;

namespace SpeechToText;

public sealed class SpeechToTextEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            SpeechToTextSessionStartedDomainEvent e => new SpeechToTextSessionStarted(e.SessionId.Value),
            SpeechTranscribedDomainEvent e => new SpeechToTextRecieved(e.SpeechToTextId.Value),
            SpeechToTextSessionCompletedDomainEvent e => new SpeechToTextResponded(e.SpeechToTextId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            SpeechToTextSessionStartedDomainEvent e => new StartSpeechToTextMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            SpeechTranscribedDomainEvent e => new SendSpeechToTextMongo(e.SessionId.Value, e.SpeechToTextId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            SpeechToTextSessionCompletedDomainEvent e => new SendSpeechToTextMongo(e.SessionId.Value, e.SpeechToTextId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            SpeechToTextSessionDeletedDomainEvent e => new UploadSpeechAudioMongo(e.Id.Value),
            _ => null
        };
    }
}