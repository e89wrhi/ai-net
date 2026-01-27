using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionStartedDomainEvent(SessionId SessionId, UserId UserId, string Title) : IDomainEvent;
