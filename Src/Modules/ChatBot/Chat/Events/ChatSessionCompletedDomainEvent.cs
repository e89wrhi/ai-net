using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionCompletedDomainEvent(SessionId Id): IDomainEvent;
