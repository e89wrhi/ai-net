using AI.Common.Core;
using AutoComplete.ValueObjects;

namespace AutoComplete.Events;

public record AutoCompleteRequestedDomainEvent(
    AutoCompleteId SessionId, 
    AutoCompleteRequestId RequestId, 
    string Prompt, 
    string Response,
    long TokensUsed) : IDomainEvent;
