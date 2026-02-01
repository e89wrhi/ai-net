using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionDeletedDomainEvent(SessionId Id): IDomainEvent;