using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record MessageReceivedDomainEvent(SessionId Id, MessageId MessageId, string Content, int TokenUsed) : IDomainEvent;
