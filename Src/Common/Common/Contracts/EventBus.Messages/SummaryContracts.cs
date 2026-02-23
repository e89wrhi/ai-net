using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record TextSummarized(Guid Id, int OriginalLength, int SummaryLength) : IIntegrationEvent;
public record SummaryRefined(Guid Id) : IIntegrationEvent;
public record KeyPointsExtracted(Guid Id, int Count) : IIntegrationEvent;
public record BulletPointsGenerated(Guid Id) : IIntegrationEvent;
