using AI.Common.Contracts.EventBus.Summarys;
using AI.Common.Core;
using Summary.Events;
using Summary.Features.DeleteSummary.V1;
using Summary.Features.SendSummary.V1;
using Summary.Features.StartSummary.V1;

namespace Summary;

public sealed class SummaryEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            TextSummarySessionStartedDomainEvent e => new SummarySessionStarted(e.SessionId.Value),
            SummaryRecievedDomainEvent e => new SummaryRecieved(e.SummaryId.Value),
            SummaryRespondedDomainEvent e => new SummaryResponded(e.SummaryId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            TextSummarySessionStartedDomainEvent e => new StartSummaryMongo(e.SessionId.Value, e.UserId.Value, e.Title, e.AiModelId, "Active", DateTime.UtcNow),
            SummaryRecievedDomainEvent e => new SendSummaryMongo(e.SessionId.Value, e.SummaryId.Value, e.Content, "User", e.TokenUsed, DateTime.UtcNow),
            SummaryRespondedDomainEvent e => new SendSummaryMongo(e.SessionId.Value, e.SummaryId.Value, e.Response, "AI", e.TokenUsed, DateTime.UtcNow),
            SummarySessionDeletedDomainEvent e => new SummarizeTextMongo(e.Id.Value),
            _ => null
        };
    }
}