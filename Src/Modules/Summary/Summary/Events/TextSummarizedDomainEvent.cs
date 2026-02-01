using AI.Common.Core;
using Summary.ValueObjects;

namespace Summary.Events;

public record TextSummarizedDomainEvent(SummaryId Id, SummaryResultId ResultId, string Summary): IDomainEvent;
