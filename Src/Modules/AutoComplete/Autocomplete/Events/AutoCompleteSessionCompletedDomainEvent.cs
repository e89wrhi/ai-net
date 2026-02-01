using AI.Common.Core;
using AutoComplete.ValueObjects;

namespace AutoComplete.Events;

public record AutoCompleteSessionCompletedDomainEvent(AutoCompleteId Id): IDomainEvent;
