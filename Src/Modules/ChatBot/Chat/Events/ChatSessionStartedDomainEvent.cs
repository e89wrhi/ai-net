using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionStartedDomainEvent(SessionId SessionId, UserId UserId, string Title) : IDomainEvent;
public record MessageRecievedDomainEvent(SessionId SessionId, MessageId MessageId, string Content) : IDomainEvent;
