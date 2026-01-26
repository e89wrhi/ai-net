using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record MessageRecievedDomainEvent(SessionId SessionId, MessageId MessageId, string Content) : IDomainEvent;
