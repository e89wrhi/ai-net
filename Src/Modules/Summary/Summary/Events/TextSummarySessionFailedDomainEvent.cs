using AI.Common.Core;
using Summary.Enums;
using Summary.ValueObjects;

namespace Summary.Events;

public record TextSummarySessionFailedDomainEvent(SummaryId Id, TextSummaryFailureReason Reason): IDomainEvent;
