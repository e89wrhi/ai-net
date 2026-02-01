using AI.Common.Core;
using AutoComplete.Enums;
using AutoComplete.ValueObjects;

namespace AutoComplete.Events;

public record AutoCompleteSessionFailedDomainEvent(AutoCompleteId Id, AutoCompleteFailureReason Reason): IDomainEvent;
