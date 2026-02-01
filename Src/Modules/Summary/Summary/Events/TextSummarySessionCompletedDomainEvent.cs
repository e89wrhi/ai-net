using AI.Common.Core;
using Summary.ValueObjects;

namespace Summary.Events;

public record TextSummarySessionCompletedDomainEvent(SummaryId Id): IDomainEvent;
