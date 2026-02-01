using AI.Common.Core;
using AutoComplete.ValueObjects;

namespace AutoComplete.Events;

public record AutoCompleteRequestedDomainEvent(AutoCompleteId Id, AutoCompleteRequestId RequestId, string Prompt): IDomainEvent;
